using Microsoft.Extensions.Logging;

namespace Fishbowl.Preflight.AspNetCore
{
    public class OpenApiValidationHandler : DelegatingHandler
    {
        private readonly HttpClientOpenApiValidator _validator;

        public OpenApiValidationHandler(HttpClientOpenApiValidator validator)
        {
            _validator = validator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                var message = string.Join("; ", result.Errors);
                throw new OpenApiValidationException(result.Errors);
            }

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
