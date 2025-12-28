# Tegla API - Exception Handling Documentation

Tegla is an ASP.NET Core Web API that demonstrates a **Layered Exception Handling Pattern** for .NET applications. This pattern is inspired by [The Standard](https://github.com/hassanhabib/The-Standard).

## Table of Contents

- [Overview](#overview)
- [Pattern Origin](#pattern-origin)
- [Architecture](#architecture)
- [Exception Hierarchy](#exception-hierarchy)
- [Core Components](#core-components)
  - [Domain Exceptions](#domain-exceptions)
  - [TryCatch Pattern](#trycatch-pattern)
  - [Validation Layer](#validation-layer)
  - [Controller Exception Handling](#controller-exception-handling)
  - [Logging Integration](#logging-integration)
- [Exception Flow](#exception-flow)
- [Industry Alternatives](#industry-alternatives)
- [Implementation Guide](#implementation-guide)
- [Best Practices](#best-practices)
- [Project Structure](#project-structure)
- [Dependencies](#dependencies)

## Overview

The Layered Exception Handling Pattern implements a **three-tier exception wrapping strategy** that:

1. **Abstracts internal errors** - End users see friendly messages, not stack traces
2. **Preserves debugging information** - Original exceptions are wrapped as inner exceptions
3. **Categorizes errors by type** - Validation, dependency, and service errors are handled differently
4. **Centralizes exception handling** - One place to manage all exception logic per service
5. **Enables appropriate logging** - Different severity levels for different exception types

## Pattern Origin

This pattern comes from **"The Standard"**, a software engineering methodology. Key characteristics:

- Used by a specific community following The Standard methodology
- Not mainstream in the broader .NET ecosystem
- Emphasizes strict layering and separation of concerns
- Uses broker pattern for infrastructure abstractions

**What's valuable to learn from this pattern:**
- Exception categorization (Validation/Dependency/Service)
- Preserving inner exceptions for debugging
- Centralized logging with appropriate severity levels
- Separation of concerns via partial classes

**What's opinionated/non-standard:**
- Delegate overloading for TryCatch (most codebases use generics or middleware)
- Partial class proliferation (3 files per service)
- Manual try-catch in every controller action (vs. global middleware)

## Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                        API Layer (Controller)                        │
│  Catches: ItemValidationException, ItemDependencyException,         │
│           ItemServiceException                                       │
│  Returns: HTTP 400 with inner exception message                     │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    Application Layer (Service)                       │
│  TryCatch() wrapper method handles all exceptions                   │
│  Maps: NullItemException      → ItemValidationException             │
│        InvalidItemException   → ItemValidationException             │
│        SqlException           → ItemDependencyException             │
│        Exception (catch-all)  → ItemServiceException                │
│  Logs with appropriate severity                                     │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     Validation Layer                                 │
│  Throws: NullItemException, InvalidItemException                    │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                   Persistence Layer (Broker)                        │
│  May throw: SqlException (from database operations)                 │
└─────────────────────────────────────────────────────────────────────┘
```

## Exception Hierarchy

The pattern uses two categories of exceptions:

### Inner Exceptions (Low-Level)

Thrown by validation and data layers, containing specific error details:

| Exception | Purpose | Message Example |
|-----------|---------|-----------------|
| `NullItemException` | Item reference is null | "Item is null." |
| `InvalidItemException` | Property validation failed | "Invalid Employee, ParameterName: Name, ParameterValue: null." |
| `SqlException` | Database operation failed | (SQL Server error) |

### Wrapper Exceptions (High-Level)

Wrap inner exceptions with user-friendly messages:

| Exception | Purpose | Message | Logging Level |
|-----------|---------|---------|---------------|
| `ItemValidationException` | Wraps validation errors | "Invalid input, contact support." | Error |
| `ItemDependencyException` | Wraps infrastructure errors | "Service dependency error occurred, contact support" | Critical |
| `ItemServiceException` | Wraps unexpected errors | "Item service error, contact support." | Error |

## Core Components

### Domain Exceptions

Located in `Tegla.Domain/Models/Items/Exceptions/`

**NullItemException** - Thrown when an item is null:

```csharp
public class NullItemException : Exception
{
    public NullItemException()
        : base("Item is null.")
    { }
}
```

**InvalidItemException** - Thrown for invalid property values:

```csharp
public class InvalidItemException : Exception
{
    public InvalidItemException(string parameterName, object parameterValue)
        : base($"Invalid Employee, " +
              $"ParameterName : {parameterName}, " +
              $"ParameterValue: {parameterValue}.")
    { }
}
```

**Wrapper Exceptions** - Always take an inner exception:

```csharp
public class ItemValidationException : Exception
{
    public ItemValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException)
    { }
}

public class ItemDependencyException : Exception
{
    public ItemDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException)
    { }
}

public class ItemServiceException : Exception
{
    public ItemServiceException(Exception innerException)
        : base("Item service error, contact support.", innerException)
    { }
}
```

### TryCatch Pattern

Located in `Tegla.Application/Services/Items/ItemService.Exceptions.cs`

The TryCatch pattern uses **delegates** to wrap business logic with standardized exception handling:

**Step 1: Define delegates for each operation type:**

```csharp
public delegate ValueTask<CreateItemResponse> ReturningAddItemFunc();
public delegate ValueTask<IEnumerable<Item>> ReturningListAllItemsFunc();
public delegate ValueTask<UpdateItemResponse> ReturningModifyItemFunc();
public delegate ValueTask<RetriveItemByIdResponse> ReturningRetriveItemByIdFunc();
public delegate ValueTask<RemoveItemByIdResponse> ReturningRemoveItemByIdFunc();
```

**Step 2: Create overloaded TryCatch methods:**

```csharp
public async ValueTask<CreateItemResponse> TryCatch(ReturningAddItemFunc returningAddItemFunc)
{
    try
    {
        return await returningAddItemFunc();
    }
    catch (NullItemException nullItemException)
    {
        throw CreateAndLogValidationException(nullItemException);
    }
    catch (InvalidItemException invalidItemException)
    {
        throw CreateAndLogValidationException(invalidItemException);
    }
    catch (SqlException sqlException)
    {
        throw CreateAndLogCriticalDependencyException(sqlException);
    }
    catch (Exception exception)
    {
        throw CreateAndLogServiceException(exception);
    }
}
```

**Step 3: Create helper methods for wrapping and logging:**

```csharp
private ItemValidationException CreateAndLogValidationException(Exception exception)
{
    var itemValidationException = new ItemValidationException(exception);
    _loggingBroker.LogError(itemValidationException);
    return itemValidationException;
}

private ItemDependencyException CreateAndLogCriticalDependencyException(Exception exception)
{
    var itemDependencyException = new ItemDependencyException(exception);
    _loggingBroker.LogCritical(itemDependencyException);
    return itemDependencyException;
}

private ItemServiceException CreateAndLogServiceException(Exception exception)
{
    var itemServiceException = new ItemServiceException(exception);
    _loggingBroker.LogError(itemServiceException);
    return itemServiceException;
}
```

**Step 4: Use TryCatch in service methods:**

```csharp
public ValueTask<CreateItemResponse> AddItem(CreateItemRequest item) =>
TryCatch(async () =>
{
    Item maybeItem = _mapper.Map<Item>(item);
    ValidateItemOnCreate(maybeItem);
    var res = await _storageBroker.InsertItem(maybeItem);
    return _mapper.Map<CreateItemResponse>(res);
});
```

### Validation Layer

Located in `Tegla.Application/Services/Items/ItemService.Validations.cs`

Validation methods throw domain exceptions for invalid states:

```csharp
public void ValidateItemOnCreate(Item item)
{
    ValidateItem(item);
    ValidateItemStrings(item);
    ValidateItemPrice(item);
}

public void ValidateItem(Item item)
{
    if (item is null)
    {
        throw new NullItemException();
    }
}

public void ValidateItemStrings(Item item)
{
    switch (item)
    {
        case { } when IsInvalid(item.Name):
            throw new InvalidItemException(
                parameterName: nameof(item.Name),
                parameterValue: item.Name);

        case { } when IsInvalid(item.Description):
            throw new InvalidItemException(
                parameterName: nameof(item.Description),
                parameterValue: item.Description);

        case { } when IsInvalid(item.Make):
            throw new InvalidItemException(
                parameterName: nameof(item.Make),
                parameterValue: item.Make);

        case { } when IsInvalid(item.Origin):
            throw new InvalidItemException(
                parameterName: nameof(item.Origin),
                parameterValue: item.Origin);
    }
}

public void ValidateItemPrice(Item item)
{
    switch (item)
    {
        case { } when IsInvalid(item.Price):
            throw new InvalidItemException(
                parameterName: nameof(item.Price),
                parameterValue: item.Price);
    }
}

public bool IsInvalid(string input) => string.IsNullOrWhiteSpace(input);
public bool IsInvalid(double input) => input < default(double);
```

### Controller Exception Handling

Located in `Tegla.API/Controllers/ItemsController.cs`

Controllers catch the three wrapper exception types and return appropriate HTTP responses:

```csharp
[HttpPost]
public async ValueTask<IActionResult> CreateItem(CreateItemRequest item)
{
    try
    {
        var res = await _itemService.AddItem(item);
        return Ok(res);
    }
    catch (ItemValidationException itemValidationException)
    {
        return BadRequest(GetMessage(itemValidationException));
    }
    catch (ItemDependencyException itemDependencyException)
    {
        return BadRequest(GetMessage(itemDependencyException));
    }
    catch (ItemServiceException itemServiceException)
    {
        return BadRequest(GetMessage(itemServiceException));
    }
}

private string GetMessage(Exception exception) =>
    exception.InnerException.Message;
```

The `GetMessage` method extracts the **inner exception's message**, providing specific error details to API consumers while keeping the wrapper message for internal logging.

### Logging Integration

Located in `Tegla.Persistence/Brokers/Loggings/LoggingBroker.cs`

The logging broker provides appropriate severity levels:

```csharp
public class LoggingBroker : ILoggingBroker
{
    private readonly ILogger _logger;

    public LoggingBroker(ILogger logger) => _logger = logger;

    public void LogCritical(Exception exception) =>
        _logger.Fatal(exception, exception.Message);

    public void LogError(Exception exception) =>
        _logger.Error(exception, exception.Message);

    // ... other logging methods
}
```

**Logging severity mapping:**

| Exception Type | Logging Method | Serilog Level |
|----------------|----------------|---------------|
| `ItemValidationException` | `LogError()` | Error |
| `ItemDependencyException` | `LogCritical()` | Fatal |
| `ItemServiceException` | `LogError()` | Error |

## Exception Flow

### Validation Error Flow

```
1. Controller receives request
2. Service.AddItem() calls TryCatch()
3. TryCatch executes business logic delegate
4. ValidateItemOnCreate() throws NullItemException
5. TryCatch catches NullItemException
6. CreateAndLogValidationException() wraps in ItemValidationException
7. LoggingBroker.LogError() logs the exception
8. ItemValidationException is thrown to controller
9. Controller catches ItemValidationException
10. GetMessage() extracts "Item is null." from inner exception
11. Controller returns BadRequest("Item is null.")
```

### Database Error Flow

```
1. Controller receives request
2. Service.AddItem() calls TryCatch()
3. TryCatch executes business logic delegate
4. StorageBroker.InsertItem() throws SqlException
5. TryCatch catches SqlException
6. CreateAndLogCriticalDependencyException() wraps in ItemDependencyException
7. LoggingBroker.LogCritical() logs with FATAL severity
8. ItemDependencyException is thrown to controller
9. Controller catches ItemDependencyException
10. GetMessage() extracts SQL error message from inner exception
11. Controller returns BadRequest with SQL error details
```

## Industry Alternatives

This pattern is one of several approaches to exception handling in .NET. Here's how it compares:

| Approach | Popularity | Pros | Cons |
|----------|------------|------|------|
| **Global Exception Middleware** | Very common | Centralized, clean controllers | Less granular control |
| **Result/Either Pattern** | Growing | No exceptions for expected failures | Learning curve, verbose |
| **FluentValidation + ProblemDetails** | Very common | Declarative, standard format | Additional dependency |
| **MediatR Pipeline Behaviors** | Common (CQRS) | Clean separation | Requires MediatR |
| **This Pattern (The Standard)** | Niche | Explicit, layered | Verbose, non-standard |

### Example: Result Pattern (Alternative)

```csharp
public class Result<T>
{
    public T Value { get; }
    public Error Error { get; }
    public bool IsSuccess => Error == null;

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(Error error) => new(default, error);
}

// Usage - no exceptions for validation
public async Task<Result<Item>> AddItem(Item item)
{
    if (item is null)
        return Result<Item>.Failure(new Error("Item is null"));

    var saved = await _repository.InsertAsync(item);
    return Result<Item>.Success(saved);
}
```

### Example: Global Middleware (Alternative)

```csharp
// Program.cs
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        // Handle and return ProblemDetails
    });
});

// Controller - no try-catch needed
[HttpPost]
public async Task<IActionResult> CreateItem(CreateItemRequest item)
{
    var res = await _itemService.AddItem(item);
    return Ok(res);
}
```

## Implementation Guide

### Adding Exception Handling to a New Entity

**1. Create domain exceptions:**

```csharp
// Domain/Models/Products/Exceptions/NullProductException.cs
public class NullProductException : Exception
{
    public NullProductException() : base("Product is null.") { }
}

// Domain/Models/Products/Exceptions/InvalidProductException.cs
public class InvalidProductException : Exception
{
    public InvalidProductException(string parameterName, object parameterValue)
        : base($"Invalid Product, ParameterName: {parameterName}, ParameterValue: {parameterValue}.")
    { }
}

// Domain/Models/Products/Exceptions/ProductValidationException.cs
public class ProductValidationException : Exception
{
    public ProductValidationException(Exception innerException)
        : base("Invalid product input, contact support.", innerException)
    { }
}

// Domain/Models/Products/Exceptions/ProductDependencyException.cs
public class ProductDependencyException : Exception
{
    public ProductDependencyException(Exception innerException)
        : base("Product dependency error occurred, contact support.", innerException)
    { }
}

// Domain/Models/Products/Exceptions/ProductServiceException.cs
public class ProductServiceException : Exception
{
    public ProductServiceException(Exception innerException)
        : base("Product service error, contact support.", innerException)
    { }
}
```

**2. Create partial service classes:**

```csharp
// ProductService.cs - Main service
public partial class ProductService : IProductService
{
    public ValueTask<Product> AddProduct(Product product) =>
    TryCatch(async () =>
    {
        ValidateProductOnCreate(product);
        return await _storageBroker.InsertProduct(product);
    });
}

// ProductService.Validations.cs
public partial class ProductService
{
    public void ValidateProductOnCreate(Product product)
    {
        ValidateProduct(product);
        // Add more validations...
    }

    public void ValidateProduct(Product product)
    {
        if (product is null)
            throw new NullProductException();
    }
}

// ProductService.Exceptions.cs
public partial class ProductService
{
    public delegate ValueTask<Product> ReturningAddProductFunc();

    public async ValueTask<Product> TryCatch(ReturningAddProductFunc func)
    {
        try
        {
            return await func();
        }
        catch (NullProductException ex)
        {
            throw CreateAndLogValidationException(ex);
        }
        catch (InvalidProductException ex)
        {
            throw CreateAndLogValidationException(ex);
        }
        catch (SqlException ex)
        {
            throw CreateAndLogCriticalDependencyException(ex);
        }
        catch (Exception ex)
        {
            throw CreateAndLogServiceException(ex);
        }
    }

    private ProductValidationException CreateAndLogValidationException(Exception ex)
    {
        var exception = new ProductValidationException(ex);
        _loggingBroker.LogError(exception);
        return exception;
    }

    private ProductDependencyException CreateAndLogCriticalDependencyException(Exception ex)
    {
        var exception = new ProductDependencyException(ex);
        _loggingBroker.LogCritical(exception);
        return exception;
    }

    private ProductServiceException CreateAndLogServiceException(Exception ex)
    {
        var exception = new ProductServiceException(ex);
        _loggingBroker.LogError(exception);
        return exception;
    }
}
```

**3. Handle in controller:**

```csharp
[HttpPost]
public async ValueTask<IActionResult> CreateProduct(Product product)
{
    try
    {
        var result = await _productService.AddProduct(product);
        return Ok(result);
    }
    catch (ProductValidationException ex)
    {
        return BadRequest(ex.InnerException.Message);
    }
    catch (ProductDependencyException ex)
    {
        return BadRequest(ex.InnerException.Message);
    }
    catch (ProductServiceException ex)
    {
        return BadRequest(ex.InnerException.Message);
    }
}
```

## Best Practices

### Do

- **Always wrap exceptions** - Never let raw exceptions propagate to consumers
- **Preserve inner exceptions** - Always pass the original exception as `innerException`
- **Use appropriate log levels** - Critical for infrastructure, Error for validation/service
- **Keep wrapper messages generic** - "Contact support" style messages for security
- **Return inner exception messages** - API consumers need specific error details
- **Use partial classes** - Separate concerns: main logic, validations, exception handling
- **Use delegates for TryCatch** - Enables type-safe exception handling per operation

### Don't

- **Don't swallow exceptions** - Always log before re-throwing
- **Don't expose stack traces** - Keep internal details out of API responses
- **Don't use generic catch-all first** - Order catches from specific to general
- **Don't skip the validation layer** - All business rules should be validated before persistence
- **Don't log and throw the same exception** - Wrap first, then log the wrapper

## Project Structure

```
Tegla/
├── Tegla.API/
│   └── Controllers/
│       └── ItemsController.cs          # Exception handling at API boundary
├── Tegla.Application/
│   └── Services/
│       └── Items/
│           ├── ItemService.cs           # Main service implementation
│           ├── ItemService.Validations.cs  # Validation methods
│           └── ItemService.Exceptions.cs   # TryCatch pattern
├── Tegla.Domain/
│   └── Models/
│       └── Items/
│           ├── Item.cs                  # Domain entity
│           └── Exceptions/
│               ├── NullItemException.cs
│               ├── InvalidItemException.cs
│               ├── ItemValidationException.cs
│               ├── ItemDependencyException.cs
│               └── ItemServiceException.cs
└── Tegla.Persistence/
    └── Brokers/
        ├── Loggings/
        │   └── LoggingBroker.cs         # Logging abstraction
        └── Storages/
            └── StorageBroker.cs         # Data access abstraction
```

## Dependencies

- **AutoMapper** - Request/response mapping
- **Serilog** - Structured logging
- **Entity Framework Core** - Data persistence
- **Microsoft.Data.SqlClient** - SQL Server connectivity

## License

This is an **educational/demonstration project** showing one approach to exception handling in .NET applications. The pattern originates from "The Standard" methodology and is not widely used in mainstream .NET development.

**Use this project to:**
- Learn about exception categorization concepts
- Understand exception wrapping and inner exception preservation
- See one approach to layered exception handling

**Do not use this project as:**
- A production-ready template
- A best practices guide for modern .NET APIs
- A standard pattern that other developers will recognize
