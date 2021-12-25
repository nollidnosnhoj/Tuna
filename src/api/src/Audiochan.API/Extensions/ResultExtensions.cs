using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Commons.Results;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.API.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToObjectResult(this Result businessResult, Func<ActionResult> successFunc)
        {
            return businessResult.Succeeded 
                ? successFunc() 
                : ((ErrorResult)businessResult).ToErrorObjectResult();
        }
        
        public static async Task<ActionResult> ToObjectResult(this Result businessResult, Func<Task<ActionResult>> successFunc)
        {
            return businessResult.Succeeded 
                ? await successFunc() 
                : ((ErrorResult)businessResult).ToErrorObjectResult();
        }
    
        public static ActionResult<T> ToObjectResult<T>(this Result<T> businessResult, Func<T, ActionResult> successFunc)
        {
            return businessResult.Succeeded 
                ? successFunc(businessResult.Data) 
                : ((ErrorResult<T>)businessResult).ToErrorObjectResult();
        }
        
        public static async Task<ActionResult<T>> ToObjectResult<T>(this Result<T> businessResult, Func<T, Task<ActionResult>> successFunc)
        {
            return businessResult.Succeeded 
                ? await successFunc(businessResult.Data) 
                : ((ErrorResult<T>)businessResult).ToErrorObjectResult();
        }
    
        public static ActionResult ToErrorObjectResult(this IErrorResult businessResult)
        {
            if (businessResult is UnauthorizedErrorResult)
                return new UnauthorizedResult();
            if (businessResult is ForbiddenErrorResult)
                return new ForbidResult();
            if (businessResult is NotFoundErrorResult)
                return new NotFoundObjectResult(new ErrorApiResponse(businessResult.Message, null));
            if (businessResult is ValidationErrorResult)
            {
                var validationErrors = (IReadOnlyCollection<ValidationError>) businessResult.Errors;
                var parsedValidationErrors = validationErrors
                    .GroupBy(x => x.Property, x => x.Message)
                    .ToDictionary(x => x.Key, x => x.ToArray());
                return new UnprocessableEntityObjectResult(
                    new ValidationErrorApiResponse(businessResult.Message, parsedValidationErrors)
                );
            }

            var parsedErrors = businessResult.Errors
                .Select(x => x.Message)
                .ToArray();
            return new BadRequestObjectResult(new ErrorApiResponse(businessResult.Message, parsedErrors));
        }
    }
}