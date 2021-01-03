using System.Collections.Generic;
using Audiochan.Core.Common.Enums;

namespace Audiochan.Core.Common.Constants
{
    public static class ErrorConstants
    {
        public static readonly Dictionary<ResultErrorCode, string> Messages = new() 
        {
            {ResultErrorCode.NotFound, "Unable to find requested resource."},
            {ResultErrorCode.Unauthorized, "You are not authorized."},
            {ResultErrorCode.Forbidden, "You are forbidden."},
            {ResultErrorCode.UnprocessedEntity, "Unable to process request. The request may be invalid."},
            {ResultErrorCode.BadRequest, "Unable to process request. Please try again later."}
        };

        public static readonly Dictionary<ResultErrorCode, string> Titles = new()
        {
            {ResultErrorCode.NotFound, "Not Found."},
            {ResultErrorCode.Unauthorized, "Unauthorized."},
            {ResultErrorCode.Forbidden, "Forbidden."},
            {ResultErrorCode.UnprocessedEntity, "Invalid Request."},
            {ResultErrorCode.BadRequest, "Invalid Request."}
        };
    }
}