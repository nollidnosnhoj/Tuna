using System;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ReturnErrorResponse(this IResult result)
        {
            var response = new ErrorViewModel(result.ToErrorTitle(), result.ToErrorMessage(), result.Errors);

            return result.ErrorCode switch
            {
                ResultStatus.NotFound => new NotFoundObjectResult(response),
                ResultStatus.Unauthorized => new UnauthorizedResult(),
                ResultStatus.Forbidden => new ForbidResult(),
                ResultStatus.UnprocessedEntity => new UnprocessableEntityObjectResult(response),
                ResultStatus.BadRequest => new BadRequestObjectResult(response),
                _ => throw new ArgumentOutOfRangeException()
            };
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
                _ => "Unable to process request."
            };
        }
        
        private static string ToErrorTitle(this IResult result)
        {
            return result.ErrorCode switch
            {
                ResultStatus.NotFound => "Not Found",
                ResultStatus.Unauthorized => "Unauthorized",
                ResultStatus.Forbidden => "Forbidden",
                ResultStatus.UnprocessedEntity => "Invalid Request",
                _ => "Invalid Request."
            };
        }
    }
}