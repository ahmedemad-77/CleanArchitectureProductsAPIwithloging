# CleanArchitectureProductsAPI (Task 4: Serilog logging)

ASP.NET Core Web API (.NET 10) using Clean Architecture, CQRS, MediatR, EF Core and SQL Server, extended with Serilog logging.

## Setup and run

Requires the .NET 10 SDK and SQL Server / LocalDB. Run from this folder:

    dotnet restore
    dotnet tool install --global dotnet-ef
    dotnet ef migrations add InitialCreate --project CleanArchitectureProductsAPI.Infrastructure --startup-project CleanArchitectureProductsAPI.API --output-dir Persistence/Migrations
    dotnet ef database update --project CleanArchitectureProductsAPI.Infrastructure --startup-project CleanArchitectureProductsAPI.API
    dotnet run --project CleanArchitectureProductsAPI.API --launch-profile https

Then open https://localhost:7080/swagger. Edit `ConnectionStrings:DefaultConnection` in
`CleanArchitectureProductsAPI.API/appsettings.json` if you are not using LocalDB
(use user-secrets or environment variables for real credentials, never commit them).

## NuGet packages added

| Project | Package | Version |
|---|---|---|
| API | Serilog.AspNetCore | 9.0.0 |
| API | Serilog.Settings.Configuration | 9.0.0 |
| API | Serilog.Sinks.Console | 6.0.0 |
| API | Serilog.Sinks.File | 6.0.0 |
| Application | Microsoft.Extensions.Logging.Abstractions | 10.0.* |

Only the API project references Serilog. Application handlers use `ILogger<T>`; Domain has no logging dependency.

## Logging configuration

Configured in the `Serilog` section of `appsettings.json` (`appsettings.Development.json` raises the app's own namespace to Debug).

- Minimum level: Information; `Microsoft.AspNetCore`, `Microsoft.EntityFrameworkCore`, `System` overridden to Warning.
- Console sink and file sink.
- Files: `CleanArchitectureProductsAPI.API/Logs/log-YYYYMMDD.txt` (relative to the working directory, which is the API project folder with `dotnet run` or Visual Studio). Daily rolling, 14 files kept, 10 MB size limit per file with roll-over.
- `Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware` is set to Fatal so the framework does not log a second copy of exceptions that `GlobalExceptionHandler` already logs.
- `app.UseSerilogRequestLogging()` is the first middleware: one event per request with method, path, status code and elapsed ms. Bodies are never logged.

## Exception handling

1. `ValidationExceptionHandler` (existing): FluentValidation errors -> 400, logged as a warning without a stack trace.
2. `GlobalExceptionHandler` (new): any other exception is logged once with full details, and the client receives a generic 500 ProblemDetails (title, trace id, no stack trace).
3. "Product not found" stays a normal 404 response, logged as a warning in `GetProductByIdHandler`.

## How to test the logging

1. Start the API and call from Swagger: `GET /api/products`, `GET /api/products/1`, `POST /api/products`.
2. `GET /api/products/9999` -> 404 and a "Product 9999 was not found" warning.
3. `POST /api/products` with `{"name":"","price":-1}` -> 400 and a validation warning.
4. Check the console and `Logs/log-*.txt` for the request events and handler messages.
5. Unexpected error: stop SQL Server / use a wrong connection string, then call `GET /api/products`. Expect a 500 ProblemDetails response and a single Error event with the stack trace in the log.
