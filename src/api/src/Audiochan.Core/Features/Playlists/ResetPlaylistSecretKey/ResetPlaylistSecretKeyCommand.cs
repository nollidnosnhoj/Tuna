using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Playlists.ResetPlaylistSecretKey
{
    public record ResetPlaylistSecretKeyCommand(long PlaylistId) : IRequest<Result<string>>;
    
    public class ResetPlaylistSecretKeyCommandHandler : IRequestHandler<ResetPlaylistSecretKeyCommand, Result<string>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INanoidGenerator _nanoidGenerator;
        private readonly long _currentUserId;

        public ResetPlaylistSecretKeyCommandHandler(ApplicationDbContext dbContext, INanoidGenerator nanoidGenerator,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _nanoidGenerator = nanoidGenerator;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<Result<string>> Handle(ResetPlaylistSecretKeyCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _dbContext.Playlists.FindAsync(
                new object[] { request.PlaylistId }, cancellationToken);
            if (playlist is null)
                return Result<string>.NotFound<Playlist>();
            if (playlist.Visibility != Visibility.Private)
                return Result<string>.NotFound<Playlist>();
            if (playlist.UserId != _currentUserId)
                return Result<string>.NotFound<Playlist>();
            playlist.Secret = await _nanoidGenerator.GenerateAsync(size: 10);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<string>.Success(playlist.Secret);
        }
    }
}