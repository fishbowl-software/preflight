using Fishbowl.Preflight.Models;
using Fishbowl.Preflight.Tests.Helpers;

namespace Fishbowl.Preflight.Tests
{
    public class PetstoreValidationTests
    {
        [Fact]
        public async Task Pet_Post_ShouldSucceed()
        {
            var validator = await OpenApiTestHelpers.LoadPetstoreValidatorAsync();

            var request = new HttpRequest
            {
                Method = "POST",
                Path = "/pet",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Body = new
                {
                    name = "Rover",
                    photoUrls = new[] { "http://example.com/photo.jpg" }
                }
        };

            var result = await validator.ValidateAsync(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Pet_Post_ShouldFail_MissingName()
        {
            var validator = await OpenApiTestHelpers.LoadPetstoreValidatorAsync();

            var request = new HttpRequest
            {
                Method = "POST",
                Path = "/pet",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Body = new
                {
                    photoUrls = new[] { "http://example.com/photo.jpg" }
                }
            };

            var result = await validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            ValidationAssert.ContainsError(result, location: "body", field: "name", messageContains: "required");
        }

        [Fact]
        public async Task Pet_Post_ShouldFail_MissinPhotoUrls()
        {
            var validator = await OpenApiTestHelpers.LoadPetstoreValidatorAsync();

            var request = new HttpRequest
            {
                Method = "POST",
                Path = "/pet",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Body = new
                {
                   name = "Rover"
                }
            };

            var result = await validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            ValidationAssert.ContainsError(result, location: "body", field: "photoUrls", messageContains: "required");
        }

        [Fact]
        public async Task Pet_Post_ShouldFail_IncorrectPath()
        {
            var validator = await OpenApiTestHelpers.LoadPetstoreValidatorAsync();

            var request = new HttpRequest
            {
                Method = "POST",
                Path = "/not-a-real-route",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Body = new
                {
                    name = "Rover",
                    photoUrls = new[] { "http://example.com/photo.jpg" }
                }
            };

            var result = await validator.ValidateAsync(request);

            Assert.False(result.IsValid);
            ValidationAssert.ContainsPathError(result, "/not-a-real-route", "No matching operation found");
        }
    }

}
