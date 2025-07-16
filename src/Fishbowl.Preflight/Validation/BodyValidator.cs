using System.Text.Json;
using Fishbowl.Preflight.Models;
using NJsonSchema;
using Newtonsoft.Json.Linq;
using NSwag;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Fishbowl.Preflight.Validation
{
    public class BodyValidator
    {
        private readonly ILogger<BodyValidator>? _logger;

        public BodyValidator(ILogger<BodyValidator>? logger = null)
        {
            _logger = logger;
        }

        public async Task ValidateAsync(object? body, OpenApiOperation operation, ValidationResult result)
        {
            _logger?.LogDebug("Starting body validation for operation: {OperationId}", operation.OperationId);

            if (body == null)
            {
                if (IsRequestBodyRequired(operation))
                {
                    _logger?.LogWarning("Request body is required but was not provided.");
                    result.AddError("body", "", "Request body is required but was not provided.");
                }
                else
                {
                    _logger?.LogDebug("No body provided, but request body is not required.");
                }

                return;
            }

            var schema = GetRequestSchema(operation);
            if (schema == null)
            {
                _logger?.LogDebug("No schema found for application/json; skipping body validation.");
                return;
            }

            var token = ToJToken(body);

            if (token == null)
            {
                _logger?.LogWarning("Body could not be converted to a valid JToken.");
                result.AddError("body", "", "Request body could not be interpreted as valid JSON.");
                return;
            }

            var errors = schema.Validate(token);

            _logger?.LogDebug("Completed schema validation: {ErrorCount} error(s) found.", errors.Count);

            foreach (var error in errors)
            {
                result.AddError("body", error.Path ?? "", error.Kind.ToString());
            }
        }

        private static JToken? ToJToken(object? body)
        {
            if (body is null)
                return null;

            if (body is JToken token)
                return token;

            if (body is string str)
            {
                str = str.Trim();

                if (string.IsNullOrWhiteSpace(str))
                    return null;

                try
                {
                    return JToken.Parse(str); // assumes it's raw JSON string
                }
                catch (JsonReaderException ex)
                {
                    throw new InvalidOperationException("Body is a string but not valid JSON.", ex);
                }
            }

            // fallback: treat it as a POCO and serialize into JToken
            return JToken.FromObject(body);
        }

        private bool IsRequestBodyRequired(OpenApiOperation operation)
        {
            return operation.RequestBody != null &&
                   operation.RequestBody.IsRequired == true;
        }

        private JsonSchema? GetRequestSchema(OpenApiOperation operation)
        {
            // Only handling application/json bodies for now
            if (operation.RequestBody?.Content.TryGetValue("application/json", out var content) == true)
            {
                return content.Schema;
            }

            return null;
        }
    }
}
