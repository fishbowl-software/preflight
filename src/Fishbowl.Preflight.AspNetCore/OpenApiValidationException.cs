using Fishbowl.Preflight.Models;

namespace Fishbowl.Preflight.AspNetCore
{
    public class OpenApiValidationException : Exception
    {
        public IEnumerable<ValidationError> Errors { get; }

        public OpenApiValidationException(IEnumerable<ValidationError> errors)
            : base("OpenAPI request validation failed.")
        {
            Errors = errors;
        }
    }
}