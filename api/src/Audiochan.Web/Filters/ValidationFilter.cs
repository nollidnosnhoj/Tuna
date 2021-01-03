using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;

namespace Audiochan.Web.Filters
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorResponse = new ErrorViewModel
                {
                    Errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(kvp => kvp.Key,
                            kvp =>
                                kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray()),
                    Title = ErrorConstants.Titles[ResultErrorCode.UnprocessedEntity],
                    Message = ErrorConstants.Titles[ResultErrorCode.UnprocessedEntity]
                };

                context.Result = new UnprocessableEntityObjectResult(errorResponse);
                return;
            }

            await next();
        }
    }
}
