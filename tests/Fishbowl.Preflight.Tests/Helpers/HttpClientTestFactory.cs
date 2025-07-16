using Fishbowl.Preflight.AspNetCore;
using Fishbowl.Preflight.Models;
using Fishbowl.Preflight.Validation;
using Microsoft.Extensions.Logging.Abstractions;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fishbowl.Preflight.Tests.Helpers
{
    public static class HttpClientTestFactory
    {
        public static HttpClient CreateValidatingHttpClient(OpenApiDocument document, Action<HttpRequest>? onValidatedRequest = null)
        {
            var requestValidator = new OpenApiRequestValidator(document);
            var clientValidator = new HttpClientOpenApiValidator(
                requestValidator,
                NullLogger<HttpClientOpenApiValidator>.Instance
            );

            var validationHandler = new OpenApiValidationHandler(clientValidator)
            {
                InnerHandler = new StubHttpHandler(onValidatedRequest)
            };

            var client = new HttpClient(validationHandler)
            {
                BaseAddress = new Uri("https://example.com")
            };

            return client;
        }

        private class StubHttpHandler : HttpMessageHandler
        {
            private readonly Action<HttpRequest>? _callback;

            public StubHttpHandler(Action<HttpRequest>? callback = null)
            {
                _callback = callback;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var body = request.Content == null ? null : await request.Content.ReadAsStringAsync();

                var internalRequest = new HttpRequest
                {
                    Method = request.Method.Method,
                    Path = request.RequestUri?.AbsolutePath ?? "",
                    Query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(request.RequestUri?.Query ?? "")
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                    Headers = request.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                    Body = body
                };

                _callback?.Invoke(internalRequest);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }
    }
}
