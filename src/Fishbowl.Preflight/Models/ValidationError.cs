namespace Fishbowl.Preflight.Models
{
    public class ValidationError
    {
        /// <summary>
        /// Where the error occurred: path, query, header, body, etc.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The specific field or parameter name, if applicable.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// A human-readable error message.
        /// </summary>
        public string Message { get; set; }
    }
}
