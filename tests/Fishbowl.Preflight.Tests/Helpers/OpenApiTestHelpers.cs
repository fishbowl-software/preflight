using Fishbowl.Preflight.Validation;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishbowl.Preflight.Tests.Helpers
{
    public static class OpenApiTestHelpers
    {
        public static async Task<OpenApiRequestValidator> LoadPetstoreValidatorAsync()
        {
            var raw = await File.ReadAllTextAsync("Schemas/petstore.json");
            var document = await OpenApiDocument.FromJsonAsync(raw);
            return new OpenApiRequestValidator(document);
        }
    }
}
