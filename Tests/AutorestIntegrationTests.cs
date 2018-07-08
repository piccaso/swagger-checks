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

            _httpClient = server.CreateClient();
            _apiClient = new MyAPI();

            // https://github.com/Azure/autorest/issues/2958
            _apiClient.GetType()
                .GetProperty(nameof(_apiClient.HttpClient))
                .SetValue(_apiClient, _httpClient);
        }

        [Test]
        public void AutoRest() {
            var value = _apiClient.ApiValuesGet();
            TestContext.WriteLine(string.Join(",", value));
            Assert.AreEqual("value2", value[1]);
        }

        [Test]
        public async Task HttpClient() {
            var values = await _httpClient.GetStringAsync("/api/Values");
            TestContext.WriteLine(values);
            Assert.IsTrue(values.Contains("value1"));
        }
    }
}