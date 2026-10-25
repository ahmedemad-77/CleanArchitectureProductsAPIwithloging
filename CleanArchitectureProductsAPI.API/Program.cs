using CleanArchitectureProductsAPI.API.Middleware;
using CleanArchitectureProductsAPI.Application;
using CleanArchitectureProductsAPI.Infrastructure;
using Microsoft.Extensions.Hosting;
using Serilog;

// Bootstrap logger: captures errors that happen before the host/configuration
// is fully built. It is replaced by the configured logger via UseSerilog below.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CleanArchitectureProductsAPI");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog replaces the default logging providers; settings come from the "Serilog"
    // section of appsettings*.json (levels, sinks, formats, retention).
    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Register services from each layer
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers();

    // Exception handlers run in registration order:
    // validation errors -> 400, anything else -> logged once + generic 500.
    builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Swagger / OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Middleware pipeline (order matters)
    // Request logging is first so it times the whole pipeline and sees the final
    // status code (including the 400/500 produced by the exception handler below).
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        // Only metadata is added; request/response bodies are never logged.
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    // HostAbortedException is thrown by EF Core design-time tools (dotnet ef); not a real failure.
    Log.Fatal(ex, "Application terminated unexpectedly during startup");
}
finally
{
    Log.CloseAndFlush();
}
