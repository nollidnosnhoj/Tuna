using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Favorites.Audios.SetFavorite
{
    public record SetFavoriteCommand(string UserId, long AudioId, bool IsFavoriting) : IRequest<IResult<bool>>
    {
    }

    public class SetFavoriteCommandHandler : IRequestHandler<SetFavoriteCommand, IResult<bool>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public SetFavoriteCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }


        public async Task<IResult<bool>> Handle(SetFavoriteCommand request, CancellationToken cancellationToken)
        {
            if (!await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == request.UserId, cancellationToken))
                return Result<bool>.Fail(ResultError.Unauthorized);

            var audio = await _dbContext.Audios
                .Include(a => a.Favorited)
                .SingleOrDefaultAsync(a => a.Id == request.AudioId, cancellationToken);

            if (audio == null)
                return Result<bool>.Fail(ResultError.NotFound);

            var favorited = request.IsFavoriting
                ? audio.AddFavorite(request.UserId)
                : audio.RemoveFavorite(request.UserId);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(favorited);
        }
    }
}