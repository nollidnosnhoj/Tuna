using Audiochan.API.Models;
using Audiochan.Core;
using Audiochan.Core.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.API.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ReturnErrorResponse(this Result result)
        {
            var response = new ErrorApiResponse(result.ToErrorCode(), result.Message, result.Errors);
            return new ObjectResult(response) {StatusCode = response.Code};
        }

        private static int ToErrorCode(this Result result)
        {
            return result.ErrorCode switch
            {
                ResultError.NotFound => StatusCodes.Status404NotFound,
                ResultError.Unauthorized => StatusCodes.Status401Unauthorized,
                ResultError.Forbidden => StatusCodes.Status403Forbidden,
                ResultError.UnprocessedEntity => StatusCodes.Status422UnprocessableEntity,
                ResultError.BadRequest => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}