using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Repositories;

namespace Audiochan.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IAudioRepository Audios { get; }
        public ITagRepository Tags { get; }
        public IUserRepository Users { get; }
        
        public void BeginTransaction()
        {
            _dbContext.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _dbContext.RollbackTransaction();
        }

        public UnitOfWork(ICurrentUserService currentUserService, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Audios = new AudioRepository(dbContext, currentUserService);
            Tags = new TagRepository(dbContext, currentUserService);
            Users = new UserRepository(dbContext, currentUserService);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}