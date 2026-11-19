using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.Http;

public static class ResultHttpExtensions
{
    public static IResult ToProblemDetails<T>(this Result<T> result)
    {
        var error = result.Error;

        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Message);
    }
}