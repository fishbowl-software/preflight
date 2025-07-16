using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Fishbowl.Preflight.Routing
{
    public class PathMatcher
    {
        private readonly ILogger<PathMatcher>? _logger;

        public PathMatcher(ILogger<PathMatcher>? logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Matches a given request path (e.g. /users/123) against a path template (e.g. /users/{id}).
        /// If it matches, extracts path parameters.
        /// </summary>
        public bool TryMatch(string template, string actualPath, out Dictionary<string, string> pathParameters)
        {
            pathParameters = new();

            var templateSegments = template.Trim('/').Split('/');
            var pathSegments = actualPath.Trim('/').Split('/');

            if (templateSegments.Length != pathSegments.Length)
            {
                _logger?.LogTrace("Path segment length mismatch: template '{Template}' vs path '{Path}'", template, actualPath);
                return false;
            }


            for (int i = 0; i < templateSegments.Length; i++)
            {
                var templateSegment = templateSegments[i];
                var pathSegment = pathSegments[i];

                if (IsParameter(templateSegment))
                {
                    var paramName = templateSegment.Trim('{', '}');
                    pathParameters[paramName] = pathSegment;
                }
                else if (!string.Equals(templateSegment, pathSegment, StringComparison.OrdinalIgnoreCase))
                {
                    _logger?.LogTrace("Segment mismatch at position {Index}: template='{Template}', path='{Path}'", i, templateSegment, pathSegment);
                    return false;
                }
            }

            _logger?.LogDebug("Successfully matched path '{ActualPath}' to template '{Template}'", actualPath, template);
            return true;
        }

        private static bool IsParameter(string segment)
        {
            return segment.StartsWith("{") && segment.EndsWith("}");
        }
    }
}
