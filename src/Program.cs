using System.Net.Http;

namespace SimpleRetryPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/users/olegburov");
            request.Headers.Add("Accept", "application/vnd.github.v3+json");
            request.Headers.Add("User-Agent", "Oleg-Burov-App");
         
            var executor = new DefaultRequestExecutor();
            var response = executor.Retry(request, 3, 3000);
        }
    }
}
