using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Enums;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.ResetPrivateKey
{
    public class ResetAudioPrivateKeyRequestHandler : IRequestHandler<ResetAudioPrivateKeyRequest, IResult<string>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public ResetAudioPrivateKeyRequestHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<string>> Handle(ResetAudioPrivateKeyRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            if (!await _dbContext.Users.AnyAsync(u => u.Id == currentUserId, cancellationToken))
                return Result<string>.Fail(ResultError.Unauthorized);
            
            var audio = await _dbContext.Audios
                .SingleOrDefaultAsync(a => a.Id == request.AudioId, cancellationToken);

            if (audio == null)
                return Result<string>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<string>.Fail(ResultError.Forbidden);

            if (string.IsNullOrEmpty(audio.PrivateKey))
                return Result<string>.Fail(ResultError.BadRequest,
                    "Audio must be set to private to change private key.");
            
            audio.SetPrivateKey();

            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(audio.PrivateKey);
        }
    }
}