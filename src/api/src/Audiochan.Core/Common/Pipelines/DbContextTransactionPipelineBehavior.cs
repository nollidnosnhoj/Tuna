using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces.Persistence;
using MediatR;

namespace Audiochan.Core.Common.Pipelines
{
    public class DbContextTransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IUnitOfWork _unitOfWork;

        public DbContextTransactionPipelineBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse? result;

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