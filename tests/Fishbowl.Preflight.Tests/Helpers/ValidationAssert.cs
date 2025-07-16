using Fishbowl.Preflight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishbowl.Preflight.Tests.Helpers
{
    public class ValidationAssert
    {
        public static void ContainsError(
           ValidationResult result,
           string location,
           string field,
           string? messageContains = null)
        {
            var match = result.Errors.Any(error =>
                string.Equals(error.Location, location, StringComparison.OrdinalIgnoreCase) &&
                error.Field.Contains(field, StringComparison.OrdinalIgnoreCase) &&
                (messageContains == null || error.Message?.Contains(messageContains, StringComparison.OrdinalIgnoreCase) == true)
            );

            if (!match)
            {
                var errors = string.Join("\n", result.Errors.Select(e => $"- [{e.Location}] {e.Field}: {e.Message}"));
                throw new Xunit.Sdk.XunitException($"Expected error at [{location}] {field} containing \"{messageContains}\" not found.\nErrors:\n{errors}");
            }
        }

        public static void ContainsPathError(ValidationResult? result, string expectedPath = null, string? messageContains = null)
        {
            var match = result.Errors.Any(error =>
                string.Equals(error.Location, "path", StringComparison.OrdinalIgnoreCase) &&
                (expectedPath == null || error.Message?.Contains(expectedPath, StringComparison.OrdinalIgnoreCase) == true) &&
                (messageContains == null || error.Message?.Contains(messageContains, StringComparison.OrdinalIgnoreCase) == true)
            );

            if (!match)
            {
                var errorDump = string.Join("\n", result.Errors.Select(e =>
                    $"- [{e.Location}] {e.Field}: {e.Message}"));

                throw new Xunit.Sdk.XunitException(
                    $"Expected path error{(expectedPath != null ? $" for path '{expectedPath}'" : "")} " +
                    $"{(messageContains != null ? $"containing \"{messageContains}\"" : "")} not found.\n\nErrors:\n{errorDump}");
            }
        }
    }
}
