using System.Collections.Generic;
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
                : Result<bool>.Invalid(message, identityResult.ToResultErrors());
        }
        
        private static Dictionary<string, List<string>> ToResultErrors(this IdentityResult identityResult)
        {
            var dict = new Dictionary<string, List<string>>();

            foreach (var identityError in identityResult.Errors)
            {
                if (!dict.ContainsKey(identityError.Code))
                    dict[identityError.Code] = new List<string>();
                
                dict[identityError.Code].Add(identityError.Description);
            }

            return dict;
        }
    }
}