using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureProductsAPI.API.Middleware;

// Last-resort handler for UNEXPECTED exceptions. It is registered after
// ValidationExceptionHandler, so validation errors never reach it.
// This is the single place where unexpected exceptions are logged.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Client disconnected: not an application failure, so don't log it as an error.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Request {RequestMethod} {RequestPath} was canceled by the client",
                httpContext.Request.Method, httpContext.Request.Path.Value);
            return true;
        }

        var traceId = httpContext.TraceIdentifier;

        // Full exception (stack trace included) goes to the logs only.
        _logger.LogError(exception,
            "Unhandled exception while processing {RequestMethod} {RequestPath} (TraceId {TraceId})",
            httpContext.Request.Method, httpContext.Request.Path.Value, traceId);

        // The client gets a generic message, no stack trace or internal details.
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = "Please try again later. If the problem persists, contact support and quote the trace id."
        };
        problem.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            problem, options: null, contentType: "application/problem+json",
            cancellationToken: cancellationToken);
        return true;
    }
}
