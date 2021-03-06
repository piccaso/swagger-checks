﻿using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Readers;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NUnit.Framework;
using RestApi.Basic;

namespace Tests
{
    [TestFixture]
    public class WebApiTests
    {
        private HttpClient _client;
        private TestServer _server;

        [SetUp]
        public void SetUp()
        {
            // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests

            var host = new WebHostBuilder().UseStartup<Startup>();
            _server = new TestServer(host);
            _client = _server.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _server.Dispose();
        }

        private async Task<string> GetSwaggerJson(string requestUri = "/swagger/v1/swagger.json") {
            var response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        [Test]
        public async Task HasSwaggerJson()
        {
            var swaggerJson = await GetSwaggerJson();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(swaggerJson));
        }

        [Test]
        public async Task CanBuildClient() {
            var swaggerJson = await GetSwaggerJson();

            var document = await SwaggerDocument.FromJsonAsync(swaggerJson);
            var settings = new SwaggerToCSharpClientGeneratorSettings {
                InjectHttpClient = true,
                GenerateSyncMethods = true,
                CSharpGeneratorSettings = {
                    Namespace = "Client",
                    ClassStyle = CSharpClassStyle.Poco,
                    GenerateJsonMethods = false,
                }
            };
            var generator = new SwaggerToCSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(code));
            File.WriteAllText("Client.cs", code, Encoding.UTF8);
        }

        [Test]
        public async Task CanReadSpec() {
            var swaggerJson = await GetSwaggerJson();
            var reader = new OpenApiStringReader();
            var document = reader.Read(swaggerJson, out var diagnostic);
            if (diagnostic.Errors.Count > 0) {
                TestContext.WriteLine("Dignostic errors:");
                foreach (var openApiError in diagnostic.Errors) {
                    TestContext.WriteLine($"{openApiError.Message} ({openApiError.Pointer})");
                }
                Assert.Fail("diagnostic errors");
            }
            Assert.AreEqual(OpenApiSpecVersion.OpenApi2_0, diagnostic.SpecificationVersion);
            Assert.IsTrue(document.Paths.Count > 0);
        }

        [Test]
        public async Task CanConvertSpec() {
            var swaggerJson = await GetSwaggerJson();
            var reader = new OpenApiStringReader();
            var document = reader.Read(swaggerJson, out _);
            var openApi3Yaml = document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml);
            var openApi3Json = document.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
            var openApi2Yaml = document.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);
            File.WriteAllText("openapi3-spec.yaml", openApi3Yaml, Encoding.UTF8);
            File.WriteAllText("openapi3-spec.json", openApi3Json, Encoding.UTF8);
            File.WriteAllText("openapi2-spec.yaml", openApi2Yaml, Encoding.UTF8);
            Assert.IsFalse(string.IsNullOrWhiteSpace(openApi3Yaml));
            Assert.IsFalse(string.IsNullOrWhiteSpace(openApi2Yaml));
            Assert.IsFalse(string.IsNullOrWhiteSpace(openApi3Json));
        }
    }
}
