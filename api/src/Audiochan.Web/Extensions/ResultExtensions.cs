using System;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ReturnErrorResponse(this IResult result)
        {
            var response = new ErrorViewModel(
                result.ToErrorTitle(),
                result.ToErrorMessage(),
                result.Errors);

            return result.ErrorCode switch
            {
                ResultErrorCode.NotFound => new NotFoundObjectResult(response),
                ResultErrorCode.Unauthorized => new UnauthorizedObjectResult(response),
                ResultErrorCode.Forbidden => new ObjectResult(response) { StatusCode = StatusCodes.Status403Forbidden },
                ResultErrorCode.UnprocessedEntity => new UnprocessableEntityObjectResult(response),
                _ => new BadRequestObjectResult(response)
            };
        }

        private static string ToErrorMessage(this IResult result)
        {
            if (!string.IsNullOrWhiteSpace(result.Message)) return result.Message;

            return result.ErrorCode switch
            {
                ResultErrorCode.Unauthorized => "You are not authorized to use this endpoint.",
                ResultErrorCode.Forbidden => "You are forbidden to use this endpoint.",
                ResultErrorCode.NotFound => "The requested resource was not found.",
                ResultErrorCode.UnprocessedEntity =>
                    "The request payload is invalid. Please check errors for more information.",
                _ => "The request is invalid."
            };
        }
        
        private static string ToErrorTitle(this IResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.ErrorCode switch
            {
                ResultErrorCode.Unauthorized => "Unauthorized",
                ResultErrorCode.Forbidden => "Forbidden",
                ResultErrorCode.NotFound => "Not Found",
                ResultErrorCode.UnprocessedEntity => "Invalid Payload",
                _ => "Invalid Request"
            };
        }
    }
}