using Fishbowl.Preflight.AspNetCore;
using Fishbowl.Preflight.Models;
using Fishbowl.Preflight.Tests.Helpers;
using Newtonsoft.Json;
using NSwag;
using System.Net;
using System.Text;
namespace Fishbowl.Preflight.Tests
{
    public class PetstoreHttpClientValidationTests
    {
        [Fact]
        public async Task ShouldThrowValidationException_WhenOperationDoesNotExist()
        {
            HttpRequest? captured = null;

            var document = await OpenApiDocument.FromUrlAsync("https://petstore3.swagger.io/api/v3/openapi.json");
            var client = HttpClientTestFactory.CreateValidatingHttpClient(document, r => captured = r);

            var request = new HttpRequestMessage(HttpMethod.Post, "/not-a-real-route")
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        name = "Rover",
                        photoUrls = new[] { "http://example.com/photo.jpg" }
                    }),
                    Encoding.UTF8,
                    "application/json")
            };

            var ex = await Assert.ThrowsAsync<OpenApiValidationException>(() => client.SendAsync(request));

            // ✅ Assert the error structure
            var result = new ValidationResult();
            foreach (var error in ex.Errors)
                result.AddError(error.Location, error.Field, error.Message);

            ValidationAssert.ContainsPathError(result, "/not-a-real-route", "No matching operation");
        }

        [Fact]
        public async Task ShouldPassValidation_WhenRequestIsValid()
        {
            // Arrange
            HttpRequest? capturedRequest = null;

            var document = await OpenApiDocument.FromUrlAsync("https://petstore3.swagger.io/api/v3/openapi.json");
            var client = HttpClientTestFactory.CreateValidatingHttpClient(document, r => capturedRequest = r);

            var request = new HttpRequestMessage(HttpMethod.Post, "/pet")
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        name = "Rover",
                        photoUrls = new[] { "http://example.com/photo.jpg" }
                    }),
                    Encoding.UTF8,
                    "application/json")
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(capturedRequest);
            Assert.Equal("POST", capturedRequest.Method);
            Assert.Equal("/pet", capturedRequest.Path);
        }

    }
}
