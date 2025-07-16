using Fishbowl.Preflight.Models;
using Fishbowl.Preflight.Validation;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Fishbowl.Preflight.AspNetCore
{
    public class HttpClientOpenApiValidator
    {
        private readonly OpenApiRequestValidator _validator;
        private readonly ILogger<HttpClientOpenApiValidator> _logger;

        public HttpClientOpenApiValidator(OpenApiRequestValidator validator, ILogger<HttpClientOpenApiValidator> logger)
        {
            _validator = validator;
            _logger = logger;
        }

        public async Task<ValidationResult> ValidateAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating outbound HTTP request: {Method} {Url}",
         request.Method, request.RequestUri);

            var body = request.Content == null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            var genericRequest = new HttpRequest
            {
                Method = request.Method.Method,
                Path = request.RequestUri?.AbsolutePath,
                Query = Microsoft.AspNetCore.WebUtilities.QueryHelpers
                            .ParseQuery(request.RequestUri?.Query ?? "")
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                Headers = request.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                Body = body
            };

            var result = await _validator.ValidateAsync(genericRequest);

            if (!result.IsValid)
            {
                _logger.LogError("OpenAPI validation failed for request: {Method} {Url}",
                    request.Method, request.RequestUri);

                foreach (var error in result.Errors)
                {
                    _logger.LogError("Validation error at {Location}, field: {Field} — {Message}",
                        error.Location,
                        error.Field ?? "(none)",
                        error.Message);
                }

                // Optional: add Debug-level body logging only if validation fails
                if (_logger.IsEnabled(LogLevel.Debug) && body is not null)
                {
                    _logger.LogDebug("Request body (raw):\n{Body}", body);
                }
            }
            else
            {
                _logger.LogInformation("OpenAPI validation passed for request: {Method} {Url}",
                    request.Method, request.RequestUri);
            }

            return result;
        }
    }
}
