using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.API.Middlewares.Pipelines
{
    /// <summary>
    /// This pipeline handles the database transaction
    /// </summary>
    /// <typeparam name="TRequest">The Request object</typeparam>
    /// <typeparam name="TResponse">The Response object</typeparam>
    public class DbTransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IUnitOfWork _unitOfWork;

        public DbTransactionPipelineBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            TResponse result;

            try
            {
                _unitOfWork.BeginTransaction();
                result = await next();
                _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                throw;
            }

            return result;
        }
    }
}