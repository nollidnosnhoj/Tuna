using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Identity
{
    public static class IdentityExtensions
    {
        public static Result<bool> ToResult(this IdentityResult identityResult, string message = "")
        {
            return identityResult.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Fail(ResultError.UnprocessedEntity, message, identityResult.ToResultErrors());
        }
        
        private static Dictionary<string, string[]> ToResultErrors(this IdentityResult identityResult)
        {
            return identityResult.Errors.ToDictionary(x => x.Code, x => new[] {x.Description});
        }
    }
}