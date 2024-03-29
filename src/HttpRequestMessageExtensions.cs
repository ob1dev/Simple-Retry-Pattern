using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace SimpleRetryPattern
{
  public static class HttpRequestMessageExtensions
  {
    public static HttpRequestMessage Clone(this HttpRequestMessage request)
    {
      var clone = new HttpRequestMessage(request.Method, request.RequestUri)
      {
        Content = request.Content.Clone(),
        Version = request.Version
      };
      
      foreach (KeyValuePair<string, object> prop in request.Properties)
      {
        clone.Properties.Add(prop);
      }
      
      foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
      {
        clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
      }

      return clone;
    }

    private static HttpContent Clone(this HttpContent content)
    {
      if (content == null) return null;

      var stream = new MemoryStream();
      content.CopyToAsync(stream).Wait();
      stream.Position = 0;

      var clone = new StreamContent(stream);
      
      foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
      {
        clone.Headers.Add(header.Key, header.Value);
      }
      
      return clone;
    }
  }
}