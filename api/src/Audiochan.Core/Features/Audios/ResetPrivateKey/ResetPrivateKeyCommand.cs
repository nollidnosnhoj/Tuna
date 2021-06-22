using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.ResetPrivateKey
{
    public record ResetPrivateKeyCommand(long Id) : IRequest<Result<string>>
    {
        
    }
    
    public class ResetPrivateKeyCommandHandler : IRequestHandler<ResetPrivateKeyCommand, Result<string>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPrivateKeyCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(ResetPrivateKeyCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var audio = await _unitOfWork.Audios.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
            if (audio == null)
                return Result<string>.Fail(ResultError.NotFound);
            
            if (audio.UserId != currentUserId || audio.Visibility != Visibility.Private)
                return Result<string>.Fail(ResultError.Forbidden);
            
            audio.ResetPrivateKey();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<string>.Success(audio.PrivateKey!);
        }
    }
}