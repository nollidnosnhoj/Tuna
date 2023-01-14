using FluentValidation;
using MediatR;

namespace Audiochan.Common.CQRS.Pipelines
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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

            throw new ValidationException(failures);
        }
    }
}