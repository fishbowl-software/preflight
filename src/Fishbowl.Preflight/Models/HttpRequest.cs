namespace Fishbowl.Preflight.Models
{

    public class HttpRequest
    {
        /// <summary>
        /// The HTTP method (e.g., GET, POST, PUT, DELETE).
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The full path relative to the API root, including any path parameters filled in (e.g., /users/123).
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Query string parameters.
        /// </summary>
        public Dictionary<string, string> Query { get; set; } = new();

        /// <summary>
        /// Header values (case-insensitive).
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new();

        /// <summary>
        /// The request body, if any. Expected to be an object that will be serialized to JSON for validation.
        /// </summary>
        public object? Body { get; set; }
    }
}
