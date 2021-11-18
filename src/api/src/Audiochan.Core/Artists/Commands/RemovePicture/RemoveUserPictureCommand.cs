using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using MediatR;

namespace Audiochan.Core.Artists.Commands
{
    [Authorize]
    public record RemoveUserPictureCommand(long UserId) : IRequest<Result>;
    
    public class RemoveUserPictureCommandHandler : IRequestHandler<RemoveUserPictureCommand, Result>
    {
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveUserPictureCommandHandler(IImageService imageService, IUnitOfWork unitOfWork)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveUserPictureCommand command, CancellationToken cancellationToken)
        {
            var artist = await _unitOfWork.Artists.FindAsync(command.UserId, cancellationToken);

            if (artist is null) return Result<ImageUploadResponse>.NotFound();

            if (artist.Id != command.UserId)
                return Result<ImageUploadResponse>.Forbidden();

            if (string.IsNullOrEmpty(artist.Picture)) return Result.Success();
            
            await _imageService.RemoveImage(AssetContainerConstants.USER_PICTURES, artist.Picture, cancellationToken);

            artist.Picture = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}