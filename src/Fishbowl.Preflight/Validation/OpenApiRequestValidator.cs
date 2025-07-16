using Fishbowl.Preflight.Models;
using Fishbowl.Preflight.Routing;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NSwag;

namespace Fishbowl.Preflight.Validation
{
    public class OpenApiRequestValidator
    {
        private readonly OpenApiDocument _document;
        private readonly OperationResolver _resolver;
        private readonly ParameterValidator _parameterValidator;
        private readonly BodyValidator _bodyValidator;
        private readonly ILogger<OpenApiRequestValidator>? _logger;

        public OpenApiRequestValidator(
            OpenApiDocument document,
            ILogger<OpenApiRequestValidator>? logger = null,
            ILogger<OperationResolver>? resolverLogger = null,
            ILogger<ParameterValidator>? parameterLogger = null,
            ILogger<BodyValidator>? bodyLogger = null)
        {
            _document = document;
            _logger = logger;

            _resolver = new OperationResolver(document, resolverLogger);
            _parameterValidator = new ParameterValidator(parameterLogger);
            _bodyValidator = new BodyValidator(bodyLogger);
        }

        public async Task<ValidationResult> ValidateAsync(HttpRequest request)
        {
            _logger?.LogDebug("Validating request: {Method} {Path}", request.Method, request.Path);

            var result = new ValidationResult();

            var (operation, pathParams) = _resolver.Resolve(request.Method, request.Path);
            if (operation == null)
            {
                var msg = $"No matching operation found for {request.Method} {request.Path}";
                _logger?.LogWarning("Validation failed: {Message}", msg);
                result.AddError("path", "", msg);
                return result;
            }

            _logger?.LogDebug("Resolved operation '{OperationId}' with path params: {PathParams}",
                operation.OperationId,
                pathParams != null ? string.Join(", ", pathParams.Select(kv => $"{kv.Key}={kv.Value}")) : "none");

            var allParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (pathParams != null)
            {
                foreach (var kv in pathParams)
                    allParams[kv.Key] = kv.Value;
            }

            foreach (var kv in request.Query)
                allParams[kv.Key] = kv.Value;

            foreach (var kv in request.Headers)
                allParams[$"header:{kv.Key}"] = kv.Value;

            _logger?.LogDebug("Validating parameters (total count: {Count})", allParams.Count);
            _parameterValidator.ValidateParameters(operation, allParams, result);

            _logger?.LogDebug("Validating request body");
            await _bodyValidator.ValidateAsync(request.Body, operation, result);

            _logger?.LogDebug("Validation complete. Errors: {ErrorCount}", result.Errors.Count);

            return result;
        }
    }
}
