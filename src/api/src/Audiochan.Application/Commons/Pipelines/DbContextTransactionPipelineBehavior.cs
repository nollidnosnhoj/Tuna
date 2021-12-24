using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Pipelines.Attributes;
using Audiochan.Application.Persistence;
using MediatR;

namespace Audiochan.Application.Commons.Pipelines
{
    public class DbContextTransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DbContextTransactionPipelineBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
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