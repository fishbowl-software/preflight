# Preflight.AspNetCore

**ASP.NET Core integration for Preflight OpenAPI validation.**

Adds automatic request validation to any `HttpClient` configured in DI using OpenAPI.

---

## Installation

```bash
dotnet add package Fishbowl.Preflight.AspNetCore
```

---

## Getting Started

### 1. Register services in `Program.cs`

```csharp
builder.Services.AddSingleton<OpenApiRequestValidator>(provider =>
{
    var document = OpenApiDocument.FromUrlAsync("https://petstore3.swagger.io/api/v3/openapi.json")
                                   .GetAwaiter().GetResult(); 
    return new OpenApiRequestValidator(document);
});

builder.Services.AddTransient<OpenApiValidationHandler>();

builder.Services.AddHttpClient<PetstoreClient>()
    .AddHttpMessageHandler<OpenApiValidationHandler>();
```

---

## What It Does

- Automatically intercepts outbound `HttpRequestMessage`
- Validates against OpenAPI document
- Throws `OpenApiValidationException` if request is invalid

---

## Handling Failures

If the request is invalid, the `OpenApiValidationHandler` throws a `OpenApiValidationException`:

```csharp
try
{
    await client.SendAsync(request);
}
catch (OpenApiValidationException ex)
{
    foreach (var error in ex.Errors)
        Console.WriteLine(error.Message);
}
```

---

## Logging Support

All components support structured logging via `ILogger<T>`. To enable full visibility, configure your app's logging settings accordingly.

---

## Test Helpers

Use `HttpClientTestFactory` in tests to mock validation and inspect outbound requests:

```csharp
var client = HttpClientTestFactory.CreateValidatingHttpClient(document, out var capturedRequest);
```

---

## Compatibility

- Compatible with .NET 6+
- Supports NSwag-generated OpenAPI v3 schemas

---

## Contributing

Pull requests welcome! We’re especially interested in:
- Additional content-type support
- Improved diagnostics and error handling
- Flexible schema sources (embedded, file, etc.)
