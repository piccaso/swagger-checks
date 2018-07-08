using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using RestApi.Basic;
using RestApi.Basic.Client.Autorest;

namespace Tests {
    [TestFixture]
    public class AutorestIntegrationTests {
        private MyAPI _apiClient;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp() {
            var host = new WebHostBuilder().UseStartup<Startup>();
            var server = new TestServer(host);
            var messageHandler = server.CreateHandler() as HttpClientHandler;
            _apiClient = new MyAPI(messageHandler);
            _httpClient = server.CreateClient();
        }

        [Test]
        public void AutoRest() {
            var value = _apiClient.ApiValuesGet(); // throws: Microsoft.Rest.HttpOperationException : Operation returned an invalid status code 'NotFound'
            TestContext.WriteLine(string.Join(",", value));
        }

        [Test]
        public async Task HttpClient() {
            var values = await _httpClient.GetStringAsync("/api/Values");
            TestContext.WriteLine(values);
            Assert.IsTrue(values.Contains("value1"));
        }
    }
}