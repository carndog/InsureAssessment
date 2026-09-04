using InsuranceDomain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InsureApi.Middleware;

public sealed class ProblemDetailsExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ProblemDetailsExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        (int status, string title, string detail) = exception switch
        {
            EntityNotFoundException =>
                (StatusCodes.Status404NotFound,
                 "Not Found",
                 exception.Message),

            ConflictException =>
                (StatusCodes.Status409Conflict,
                 "Conflict",
                 exception.Message),

            DomainRuleException =>
                (StatusCodes.Status400BadRequest,
                 "Bad Request",
                 exception.Message),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "Internal Server Error",
                 "An unexpected error occurred.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail,
                    Instance = httpContext.Request.Path,
                    Type = "about:blank"
                }
            });
    }
}