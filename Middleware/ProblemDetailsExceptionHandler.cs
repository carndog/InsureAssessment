using InsuranceDomain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InsureApi.Middleware;

public sealed class ProblemDetailsExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        (int status, string title) = exception switch
        {
            EntityNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            DomainRuleException => (StatusCodes.Status400BadRequest, "Bad Request"),
            _ => (StatusCodes.Status500InternalServerError, "An error occurred")
        };

        ProblemDetails problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message,
            Type = status switch
            {
                StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc9110#section-6.5.4",
                StatusCodes.Status409Conflict => "https://tools.ietf.org/html/rfc9110#section-6.5.8",
                StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc9110#section-6.5.1",
                _ => "https://tools.ietf.org/html/rfc9110#section-6.6.1"
            }
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
