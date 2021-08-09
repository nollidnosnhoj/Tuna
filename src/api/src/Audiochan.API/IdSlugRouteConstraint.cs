using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Audiochan.API
{
    public class IdSlugRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out var value)) return false;
            var parameterValueString = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (parameterValueString is null) return false;

            if (long.TryParse(parameterValueString, out _)) return true;
            
            var splits = parameterValueString.Split('-');
            var idString = splits[0];
                
            // TODO: Check if slugString is valid slug format
            var slugString = string.Join('-', splits[1..]);
                
            return long.TryParse(idString, out _);
        }
    }
}