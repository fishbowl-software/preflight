# Preflight

Preflight is a .NET library developed by Fishbowl Software for validating and enforcing OpenAPI 3.0+ conformance. It includes a core validation engine, ASP.NET Core integration, and a test suite to support runtime enforcement and continuous integration pipelines.

## Solution Overview

This repository contains a multi-project .NET solution:

- **Fishbowl.Preflight**  
  The core library that loads and validates OpenAPI documents. It exposes structured validation results and can be used standalone or embedded into tools and pipelines.

- **Fishbowl.Preflight.AspNetCore**  
  An ASP.NET Core integration layer. It includes middleware for runtime request/response validation, startup extensions, and action filter support.

- **Fishbowl.Preflight.Tests**  
  Unit and integration tests for all validation logic, including OpenAPI spec examples, boundary cases, and ASP.NET Core handler tests.

## Features

- Load OpenAPI 3.0+ documents from YAML or JSON
- Validate structural and semantic correctness
- Emit structured validation results with diagnostics
- Middleware support for validating runtime traffic
- Designed for CI/CD enforcement
- Extensible validation pipeline

## Installation

Add the core package:

```
dotnet add package Fishbowl.Preflight
```

For ASP.NET Core middleware support:

```
dotnet add package Fishbowl.Preflight.AspNetCore
```

## Basic Usage

### Static Validation

```csharp
var doc = OpenApiDocumentLoader.Load("openapi.yaml");
var result = OpenApiValidator.Validate(doc);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"[{error.Code}] {error.Message}");
    }
}
```

### ASP.NET Core Middleware

```csharp
app.UsePreflight(options =>
{
    options.LoadFromFile("openapi.yaml");
});
```

## Running Tests

From the repository root:

```
dotnet test Fishbowl.Preflight.sln
```

## Folder Structure

```
/src
  /Fishbowl.Preflight
  /Fishbowl.Preflight.AspNetCore
/tests
  /Fishbowl.Preflight.Tests
```

## License

Copyright © Fishbowl Software.
All rights reserved.
