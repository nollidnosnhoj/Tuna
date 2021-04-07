using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Models
{
    public record ErrorViewModel(int Code, string Message, IDictionary<string, string[]> Errors)
    {
        public static ErrorViewModel NotFound(string message)
        {
            return new(StatusCodes.Status404NotFound, message, null);
        }

        public static ErrorViewModel Unauthorized(string message)
        {
            return new(StatusCodes.Status401Unauthorized, message, null);
        }
    }
}