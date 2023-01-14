using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.CQRS;
using Audiochan.Core.Persistence.Pipelines.Attributes;
using MediatR;

namespace Audiochan.Core.Persistence.Pipelines
{
    public class DbContextTransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DbContextTransactionPipelineBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse? result;

            // If the command has an explicit transaction attribute, skip the transaction pipeline,
            // as the command will handle the transaction explicitly.
            var attrs = request.GetType().GetCustomAttributes<ExplicitTransactionAttribute>();
            if (attrs.Any()) return await next();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                result = await next();

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return result;
        }
    }
}