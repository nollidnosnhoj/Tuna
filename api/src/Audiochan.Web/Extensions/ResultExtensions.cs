using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ReturnErrorResponse(this IResult result)
        {
            var response = new ErrorViewModel(result.ToErrorCode(), result.ToErrorMessage(), result.Errors);
            return new ObjectResult(response) {StatusCode = response.Code};
        }

        private static string ToErrorMessage(this IResult result)
        {
            if (!string.IsNullOrWhiteSpace(result.Message)) return result.Message;

            return result.ErrorCode switch
            {
                ResultStatus.NotFound => "The requested resource was not found.",
                ResultStatus.Unauthorized => "You are not authorized access.",
                ResultStatus.Forbidden => "You are authorized, but forbidden access.",
                ResultStatus.UnprocessedEntity => "The request payload is invalid.",
                ResultStatus.BadRequest => "Unable to process request.",
                _ => "An unknown error has occurred."
            };
        }

        private static int ToErrorCode(this IResult result)
        {
            return result.ErrorCode switch
            {
                ResultStatus.Success => StatusCodes.Status200OK,
                ResultStatus.Unauthorized => StatusCodes.Status401Unauthorized,
                ResultStatus.Forbidden => StatusCodes.Status403Forbidden,
                ResultStatus.UnprocessedEntity => StatusCodes.Status422UnprocessableEntity,
                ResultStatus.BadRequest => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}