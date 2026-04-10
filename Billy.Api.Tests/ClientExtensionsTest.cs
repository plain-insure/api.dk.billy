using System.Net;
using System.Text;
using Billy.Api.Utils;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class ClientExtensionsTest
    {
        [TestMethod]
        public async Task CreateBillyClient_WithDebugLog_LogsRequestAndResponseBodies()
        {
            var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ok\":true}", Encoding.UTF8, "application/json")
            });
            var httpClient = new HttpClient(handler);
            var debugLog = new BillyDebugLog();
            var client = ClientExtensions.CreateBillyClient(httpClient, "test-key", debugLog);

            var request = new RestRequest("contacts", Method.Post);
            request.AddJsonBody(new { foo = "bar" });

            var response = await client.ExecuteAsync(request);
            var logText = debugLog.ToString();

            Assert.IsTrue(response.IsSuccessful);
            StringAssert.Contains(logText, "POST");
            StringAssert.Contains(logText, "\"foo\":\"bar\"");
            StringAssert.Contains(logText, "{\"ok\":true}");
        }

        private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(handler(request));
        }
    }
}
