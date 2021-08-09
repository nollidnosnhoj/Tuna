using System;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Audiochan.API
{
    public class IdSlugModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext? bindingContext)
        {
            if (bindingContext is null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            
            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // If the value is a bigint, we can assume that it's the id of the resource.
            if (long.TryParse(value, out var id))
            {
                bindingContext.Result = ModelBindingResult.Success(new IdSlug(id, ""));
                return Task.CompletedTask;
            }

            // Separate id and slug
            var splits = value.Split('-');
            var idString = splits[0];
            var slugString = string.Join('-', splits[1..]);

            // Invalidate if the first portion of the slug is not a bigint
            if (!long.TryParse(idString, out id))
            {
                bindingContext.ModelState.TryAddModelError(modelName, "Invalid slug format.");
                return Task.CompletedTask;
            }
            
            bindingContext.Result = ModelBindingResult.Success(new IdSlug(id, slugString));
            return Task.CompletedTask;
        }
    }
}