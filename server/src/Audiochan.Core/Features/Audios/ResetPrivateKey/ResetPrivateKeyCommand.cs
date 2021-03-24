using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.ResetPrivateKey
{
    public record ResetPrivateKeyCommand(long AudioId) : IRequest<IResult<string>>
    {
        
    }
    
    public class ResetPrivateKeyCommandHandler : IRequestHandler<ResetPrivateKeyCommand, IResult<string>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public ResetPrivateKeyCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<string>> Handle(ResetPrivateKeyCommand request, CancellationToken cancellationToken)
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