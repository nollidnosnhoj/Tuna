using System.Linq;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Core.Enums;
using Audiochan.Core.Models.Responses;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Audiochan.API.Filters
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key.ToLower(), kvp =>
                        kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray());
                var result = Result<bool>.Fail(ResultError.UnprocessedEntity, string.Empty, errors);
                context.Result = result.ReturnErrorResponse();
                return;
            }

            await next();
        }
    }
}