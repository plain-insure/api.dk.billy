using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using RestSharp;
using RestSharp.Interceptors;

namespace Billy.Api.Utils
{
    internal sealed class BillyHttpDebugInterceptor(Action<string> writeLog) : Interceptor
    {
        public override async ValueTask BeforeHttpRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            writeLog(await FormatRequest(request, cancellationToken));
        }

        public override async ValueTask AfterHttpRequest(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            writeLog(await FormatResponse(response, cancellationToken));
        }

        private static async Task<string> FormatRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"> {request.Method} {request.RequestUri}");
            AppendHeaders(builder, request.Headers);
            if (request.Content is not null)
            {
                AppendHeaders(builder, request.Content.Headers);
                builder.AppendLine();
                builder.Append(await request.Content.ReadAsStringAsync(cancellationToken));
            }

            var result = builder.ToString();
            Console.WriteLine(result);
            return result;
        }

        private static async Task<string> FormatResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"< {(int)response.StatusCode} {response.ReasonPhrase}");
            AppendHeaders(builder, response.Headers);
            if (response.Content is not null)
            {
                AppendHeaders(builder, response.Content.Headers);
                builder.AppendLine();
                builder.Append(await response.Content.ReadAsStringAsync(cancellationToken));
            }

            var result = builder.ToString();
            Console.WriteLine(result);
            return result;
        }

        private static void AppendHeaders(StringBuilder builder, HttpHeaders headers)
        {
            foreach (var header in headers)
            {
                builder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
        }
    }
}
