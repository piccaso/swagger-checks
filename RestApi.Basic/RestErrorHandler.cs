using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RestApi.Basic {
    public class RestErrorHandler {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RestErrorHandler(RequestDelegate next, ILogger<RestErrorHandler> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            }
            catch (Exception ex) {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex) {
            _logger.LogError(ex, "unhandled exception");
            var errors = new List<RestErrorResponse> {
                new RestErrorResponse {
                    ErrorMessage = ex.Message
                }
            };
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errors));
        }

        private class RestErrorResponse {
            public string ErrorMessage { get; set; }
        }

        internal class DefaultResponseTypeFilter : IOperationFilter {
            public void Apply(Operation operation, OperationFilterContext context) {
                var schema = context.SchemaRegistry.GetOrRegister(typeof(List<RestErrorResponse>));
                operation.Responses.Add("default", new Response {
                    Description = "Errors",
                    Schema = schema
                });
            }
        }
    }
}
