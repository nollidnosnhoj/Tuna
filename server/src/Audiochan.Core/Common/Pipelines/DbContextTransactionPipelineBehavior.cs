using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using MediatR;

namespace Audiochan.Core.Common.Pipelines
{
    /// <summary>
    /// This pipeline handles the database transaction
    /// </summary>
    /// <typeparam name="TRequest">The Request object</typeparam>
    /// <typeparam name="TResponse">The Response object</typeparam>
    public class DbContextTransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IApplicationDbContext _dbContext;

        public DbContextTransactionPipelineBehavior(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            TResponse result;

            try
            {
                _dbContext.BeginTransaction();
                result = await next();
                _dbContext.CommitTransaction();
            }
            catch (Exception)
            {
                _dbContext.RollbackTransaction();
                throw;
            }

            return result;
        }
    }
}