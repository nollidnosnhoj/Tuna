using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Common.Pipelines
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : Result, new()
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators.Any()) return await next();
            
            var validationContext = new ValidationContext<TRequest>(request);
            
            var validationResults =
                await Task.WhenAll(_validators.Select(v => 
                    v.ValidateAsync(validationContext, cancellationToken)));
            
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();
            
            if (failures.Count == 0) return await next();
            
            var errors = new Dictionary<string, List<string>>();
            
            foreach (var validationFailure in failures)
            {
                if (!errors.ContainsKey(validationFailure.PropertyName))
                {
                    errors.Add(validationFailure.PropertyName, new List<string>());
                }
                        
                errors[validationFailure.PropertyName].Add(validationFailure.ErrorMessage);
            }

            return (TResponse) Result.Invalid(errors: errors);
        }
    }
}