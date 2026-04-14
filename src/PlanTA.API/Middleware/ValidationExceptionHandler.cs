using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace PlanTA.API.Middleware;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            // Diagnóstico temporal: devolver detalles completos para debuggear en producción
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Type = exception.GetType().FullName,
                Title = exception.Message,
                Status = 500,
                Inner = exception.InnerException?.Message,
                Stack = exception.StackTrace?.Split('\n').Take(15).ToArray()
            }, cancellationToken);
            return true;
        }

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            Type = "ValidationFailure",
            Title = "Validation error",
            Status = 400,
            Errors = errors
        }, cancellationToken);

        return true;
    }
}
