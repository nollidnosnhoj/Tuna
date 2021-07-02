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
        public IPlaylistRepository Playlists { get; }
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

        public UnitOfWork(ApplicationDbContext dbContext, 
            IAudioRepository audioRepository, 
            ITagRepository tagRepository, 
            IUserRepository userRepository)
        {
            _dbContext = dbContext;
            Audios = audioRepository;
            Tags = tagRepository;
            Users = userRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}