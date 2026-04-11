using Microsoft.AspNetCore.Http;

namespace PlanTA.SharedKernel.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, int successStatusCode = 200)
    {
        if (result.IsSuccess)
        {
            return successStatusCode switch
            {
                201 => Results.Created($"/{result.Value}", result.Value),
                204 => Results.NoContent(),
                _ => Results.Ok(result.Value)
            };
        }

        return result.Error!.Type switch
        {
            ErrorType.NotFound => Results.NotFound(new { result.Error.Code, result.Error.Message }),
            ErrorType.Conflict => Results.Conflict(new { result.Error.Code, result.Error.Message }),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.Internal => Results.StatusCode(500),
            _ => Results.BadRequest(new { result.Error.Code, result.Error.Message })
        };
    }
}
