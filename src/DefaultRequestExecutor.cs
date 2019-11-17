using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Linq;

namespace SimpleRetryPattern
{
    public class DefaultRequestExecutor
    {
        private readonly HttpClient _httpClient = new HttpClient();
        
        public HttpResponseMessage Retry(HttpRequestMessage request, int maxRetries, int requestTimeout)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(requestTimeout);
            var token = source.Token;

            HttpResponseMessage response;

            while (true)
            {
                var task = SendAsync(request.Clone(), token);
                task.Wait(token);

                response = task.Result;
                
                if (response.StatusCode == HttpStatusCode.TooManyRequests && --maxRetries > 0)
                {
                    var backoffDelay = CalculateBackoffDelay(response);
                    Thread.Sleep(backoffDelay);

                    continue;
                }

                break;
            }

            return response;
        }

        private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(request, cancellationToken);
        }

        private int CalculateBackoffDelay(HttpResponseMessage response)
        {
            var limitReset = response.Headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault();
            var date = response.Headers.GetValues("Date").FirstOrDefault();

            return int.Parse(limitReset) - int.Parse(date) + 1;
        }
    }
}