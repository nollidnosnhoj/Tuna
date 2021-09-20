using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Models
{
    public record ErrorApiResponse(int Code, string? Message, IDictionary<string, List<string>>? Errors)
    {
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