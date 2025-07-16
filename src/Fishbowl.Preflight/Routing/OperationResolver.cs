using NSwag;
using Microsoft.Extensions.Logging;

namespace Fishbowl.Preflight.Routing
{
    public class OperationResolver
    {
        private readonly OpenApiDocument _document;
        private readonly PathMatcher _pathMatcher;
        private readonly ILogger<OperationResolver>? _logger;

        public OperationResolver(OpenApiDocument document, ILogger<OperationResolver>? logger, ILoggerFactory? loggerFactory = null)
        {
            _document = document;
            _logger = logger;
            _pathMatcher = new PathMatcher(loggerFactory?.CreateLogger<PathMatcher>());
        }

        public (OpenApiOperation? Operation, Dictionary<string, string>? PathParams) Resolve(string method, string path)
        {
            _logger?.LogDebug("Resolving operation for method {Method} and path {Path}", method, path);

            foreach (var pathTemplate in _document.Paths.Keys)
            {
                _logger?.LogTrace("Matched template: {Template} with path: {Path}", pathTemplate, path);

                if (_pathMatcher.TryMatch(pathTemplate, path, out var pathParams))
                {
                    var pathItem = _document.Paths[pathTemplate];
                    var normalizedMethod = method.ToLowerInvariant();

                    if (pathItem.TryGetValue(normalizedMethod, out var operation))
                    {
                        _logger?.LogDebug("Resolved operation for {Method} {Path}", method, pathTemplate);
                        return (operation, pathParams);
                    }
                }

                _logger?.LogTrace("Template matched but no operation for HTTP method {Method}", method);
            }

            _logger?.LogWarning("No matching OpenAPI operation found for {Method} {Path}", method, path);
            return (null, null);
        }
    }
}
