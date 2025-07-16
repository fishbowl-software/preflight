namespace Fishbowl.Preflight.Models
{
    public class ValidationResult
    {
        /// <summary>
        /// Indicates whether the request passed all validation checks.
        /// </summary>
        public bool IsValid => !Errors.Any();

        /// <summary>
        /// A collection of all validation errors found.
        /// </summary>
        public List<ValidationError> Errors { get; } = new();

        /// <summary>
        /// Adds a new error to the result.
        /// </summary>
        public void AddError(string location, string field, string message)
        {
            Errors.Add(new ValidationError
            {
                Location = location,
                Field = field,
                Message = message
            });
        }

        /// <summary>
        /// Adds a preconstructed error.
        /// </summary>
        public void AddError(ValidationError error)
        {
            Errors.Add(error);
        }
    }
}
