# Preflight

**OpenAPI request validation for .NET applications.**  
Validates outbound HTTP requests against an OpenAPI 3.0+ document before they're sent.

---

## Features

- Validates HTTP method, path, query, headers, and body using an OpenAPI schema
- Strongly typed `ValidationResult` with detailed error locations
- Seamless integration with `HttpClient`
- Extensible architecture with logging hooks

---

## Installation

```bash
dotnet add package Fishbowl.Preflight
```

---

## Usage

### 1. Load your OpenAPI document

```csharp
var document = await OpenApiDocument.FromUrlAsync("https://petstore3.swagger.io/api/v3/openapi.json");
```

### 2. Create a validator

```csharp
var validator = new OpenApiRequestValidator(document);
```

### 3. Validate a request

```csharp
var request = new HttpRequest
{
    Method = "POST",
    Path = "/pet",
    Headers = new Dictionary<string, string>
    {
        { "Content-Type", "application/json" }
    },
    Body = new {
        photoUrls = new[] { "http://example.com/photo.jpg" }
    }
};

var result = await validator.ValidateAsync(request);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Location}.{error.Field}: {error.Message}");
    }
}
```

---

## Validation Behavior

- Parameter validation checks path, query, and headers
- Request body is parsed with `Newtonsoft.Json` and matched against the schema
- Errors include detailed paths and failure reasons

---

## Logging

If constructed with `ILogger<T>`, internal validators will emit rich logs using `Microsoft.Extensions.Logging`.

---

## Testing

Includes test helpers for:
- Real-world schema validation
- Integration with `HttpClient`
- Mock HTTP response capture

---

## Extend or Contribute

Open a PR or issue to discuss custom body format support, form-data, or advanced logging.