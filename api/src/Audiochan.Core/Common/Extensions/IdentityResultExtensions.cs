using System.Linq;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Common.Extensions
{
    public static class IdentityResultExtensions
    {
        public static IResult ToResult(this IdentityResult identityResult, string message = "")
        {
            if (identityResult.Succeeded) return Result.Success();

            var errors = identityResult.Errors.ToDictionary(
                x => x.Code,
                x => new[] {x.Description});
               
            return Result.Fail(ResultErrorCode.UnprocessedEntity, message, errors);
        }
        
        public static IResult<T> ToResult<T>(this IdentityResult identityResult, string message = "")
        {
            var errors = identityResult.Errors.ToDictionary(
                x => x.Code,
                x => new[] {x.Description});

            return Result<T>.Fail(ResultErrorCode.UnprocessedEntity, message, errors);
        }
    }
}