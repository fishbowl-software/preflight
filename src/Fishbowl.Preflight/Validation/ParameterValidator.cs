using Fishbowl.Preflight.Models;
using Microsoft.Extensions.Logging;
using NSwag;

namespace Fishbowl.Preflight.Validation
{
    public class ParameterValidator
    {
        private readonly ILogger<ParameterValidator>? _logger;

        public ParameterValidator(ILogger<ParameterValidator>? logger = null)
        {
            _logger = logger;
        }

        public void ValidateParameters(OpenApiOperation operation, Dictionary<string, string> inputParams, ValidationResult result)
        {
            foreach (var parameter in operation.Parameters)
            {
                // Skip body-type parameters — these belong to RequestBody
                if (parameter.Kind == OpenApiParameterKind.Body)
                {
                    _logger?.LogDebug("Skipping body parameter: {ParameterName}", parameter.Name);
                    continue;
                }

                var key = ResolveKey(parameter);

                // Check if required parameter is missing
                if (parameter.IsRequired && !inputParams.ContainsKey(key))
                {
                    _logger?.LogWarning("Missing required parameter: {Kind} {Name}", parameter.Kind, parameter.Name);
                    result.AddError(parameter.Kind.ToString().ToLower(), parameter.Name, "Parameter is required but was not provided.");
                    continue;
                }

                // Skip if not provided
                if (!inputParams.TryGetValue(key, out var value))
                {
                    _logger?.LogDebug("Optional parameter not provided: {Kind} {Name}", parameter.Kind, parameter.Name);
                    continue;
                }

                // Type checking
                if (!ValidateType(value, parameter.Schema?.ActualSchema?.Type))
                {
                    _logger?.LogWarning("Invalid parameter type: {Kind} {Name} = {Value}. Expected: {ExpectedType}", parameter.Kind, parameter.Name, value, parameter.Schema?.Type);
                    result.AddError(parameter.Kind.ToString().ToLower(), parameter.Name, $"Parameter value '{value}' is not valid for expected type '{parameter.Schema?.Type}'.");
                }
                else
                {
                    _logger?.LogDebug("Parameter validated: {Kind} {Name} = {Value}", parameter.Kind, parameter.Name, value);
                }
            }
        }

        private static string ResolveKey(OpenApiParameter parameter)
        {
            return parameter.Kind switch
            {
                OpenApiParameterKind.Path => parameter.Name,
                OpenApiParameterKind.Query => parameter.Name,
                OpenApiParameterKind.Header => $"header:{parameter.Name}",
                _ => parameter.Name
            };
        }

        private static bool ValidateType(string value, NJsonSchema.JsonObjectType? type)
        {
            if (type == null) return true;

            return type switch
            {
                NJsonSchema.JsonObjectType.Boolean => bool.TryParse(value, out _),
                NJsonSchema.JsonObjectType.Integer => int.TryParse(value, out _),
                NJsonSchema.JsonObjectType.Number => double.TryParse(value, out _),
                NJsonSchema.JsonObjectType.String => true,
                _ => true
            };
        }

    }
}
