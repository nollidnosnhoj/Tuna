using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Common.Extensions
{
    public static class IdentityResultExtensions
    {
        public static IResult<bool> ToResult(this IdentityResult identityResult, string message = "")
        {
            return identityResult.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Fail(ResultError.UnprocessedEntity, message,
                    identityResult.FromIdentityToResultErrors());
        }

        public static IResult<TResponse> ToResult<TResponse>(this IdentityResult identityResult, TResponse data,
            string message = "")
        {
            return identityResult.Succeeded
                ? Result<TResponse>.Success(data)
                : Result<TResponse>.Fail(ResultError.UnprocessedEntity, message,
                    identityResult.FromIdentityToResultErrors());
        }

        /// <summary>
        /// Transform the Identity Result Errors into Application Result Errors
        /// </summary>
        /// <param name="identityResult">Identity Result</param>
        /// <returns></returns>
        private static Dictionary<string, string[]> FromIdentityToResultErrors(this IdentityResult identityResult)
        {
            return identityResult.Errors.ToDictionary(x => x.Code, x => new[] {x.Description});
        }
    }
}