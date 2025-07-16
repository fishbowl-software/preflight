using Fishbowl.Preflight.AspNetCore;
using Fishbowl.Preflight.Models;

namespace Fishbowl.Preflight.Tests.Helpers
{
    public static class ValidationTestHelper
    {
        public static async Task<ValidationResult> ExpectValidationException(HttpClient client, HttpRequestMessage request)
        {
            var ex = await Assert.ThrowsAsync<OpenApiValidationException>(() => client.SendAsync(request));
            var result = new ValidationResult();

            foreach (var e in ex.Errors)
                result.AddError(e.Location, e.Field, e.Message);

            return result;
        }
    }
}
