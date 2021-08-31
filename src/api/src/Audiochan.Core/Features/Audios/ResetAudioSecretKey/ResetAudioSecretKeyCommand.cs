using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;

namespace Audiochan.Core.Features.Audios.ResetAudioSecretKey
{
    public record ResetAudioSecretKeyCommand(long AudioId) : IRequest<Result<string>>;
    
    public class ResetAudioSecretKeyCommandHandler : IRequestHandler<ResetAudioSecretKeyCommand, Result<string>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly long _currentUserId;

        public ResetAudioSecretKeyCommandHandler(ApplicationDbContext dbContext, IRandomIdGenerator randomIdGenerator, 
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _randomIdGenerator = randomIdGenerator;
            _currentUserId = currentUserService.GetUserId();
        }

        public async Task<Result<string>> Handle(ResetAudioSecretKeyCommand request, CancellationToken cancellationToken)
        {
            var audio = await _dbContext.Audios.FindAsync(new object[] { request.AudioId }, cancellationToken);
            if (audio is null)
                return Result<string>.NotFound<Audio>();
            if (audio.Visibility != Visibility.Private)
                return Result<string>.NotFound<Audio>();
            if (audio.UserId != _currentUserId)
                return Result<string>.NotFound<Audio>();
            audio.Secret = await _randomIdGenerator.GenerateAsync(size: 10);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<string>.Success(audio.Secret);
        }
    }
}