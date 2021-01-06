using System;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ReturnErrorResponse(this IResult result)
        {
            var response = new ErrorViewModel(result.ErrorCode.ToErrorTitleString(), result.GetErrorMessage(),
                result.Errors);

            return result.ErrorCode switch
            {
                ResultErrorCode.NotFound => new NotFoundObjectResult(response),
                ResultErrorCode.Unauthorized => new UnauthorizedResult(),
                ResultErrorCode.Forbidden => new ForbidResult(),
                ResultErrorCode.UnprocessedEntity => new UnprocessableEntityObjectResult(response),
                ResultErrorCode.BadRequest => new BadRequestObjectResult(response),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string GetErrorMessage(this IResult result)
        {
            return !string.IsNullOrWhiteSpace(result.Message) 
                ? result.Message 
                : ErrorConstants.Messages[result.ErrorCode ?? ResultErrorCode.BadRequest];
        }
        
        private static string ToErrorTitleString(this ResultErrorCode? status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));
            
            return ErrorConstants.Titles[(ResultErrorCode) status];
        }
    }
}