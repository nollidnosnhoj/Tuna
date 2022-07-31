using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Models
{
    public record ErrorApiResponse(int Code, string? Message, IDictionary<string, List<string>>? Errors)
    {
        public static ErrorApiResponse Invalid(IEnumerable<ValidationFailure> failures)
        {
            var dict = new Dictionary<string, List<string>>();
            foreach (var failure in failures)
            {
                if (!dict.ContainsKey(failure.PropertyName))
                {
                    dict[failure.PropertyName] = new List<string>();
                }
                
                dict[failure.PropertyName].Add(failure.ErrorMessage);
            }

            return new ErrorApiResponse(
                StatusCodes.Status422UnprocessableEntity, 
                "Validation error(s) has occurred.",
                Errors: dict);
        }
        
        public static ErrorApiResponse NotFound(string message)
        {
            return new(StatusCodes.Status404NotFound, message, null);
        }

        public static ErrorApiResponse Unauthorized()
        {
            return new(StatusCodes.Status401Unauthorized, "You are not authorized.", null);
        }

        public static ErrorApiResponse Forbidden()
        {
            return new ErrorApiResponse(StatusCodes.Status403Forbidden, "You are forbidden access.", null);
        }
    }
}