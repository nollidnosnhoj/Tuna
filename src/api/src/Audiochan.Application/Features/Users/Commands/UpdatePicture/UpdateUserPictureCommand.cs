using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Services;
using MediatR;

namespace Audiochan.Application.Features.Users.Commands.UpdatePicture
{
    public record UpdateUserPictureCommand(long UserId, string Data = "") : IImageData,
        ICommandRequest<ImageUploadResponse>;

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, ImageUploadResponse>
    {
        private readonly IImageService _imageService;
        private readonly long _currentUserId;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRandomIdGenerator _randomIdGenerator;

        public UpdateUserPictureCommandHandler(IImageService imageService,
            ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork, 
            IRandomIdGenerator randomIdGenerator)
        {
            _imageService = imageService;
            currentUserService.User.TryGetUserId(out _currentUserId);
            _unitOfWork = unitOfWork;
            _randomIdGenerator = randomIdGenerator;
        }

        public async Task<ImageUploadResponse> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user!.Id != _currentUserId)
                throw new ForbiddenException();
        
            var blobName = string.Empty;
            if (string.IsNullOrEmpty(command.Data))
            {
                await RemoveOriginalPicture(user.Picture, cancellationToken);
                user.Picture = null;
            }
            else
            {
                blobName = $"{await _randomIdGenerator.GenerateAsync(size: 15)}.jpg";
                await _imageService.UploadImage(command.Data, AssetContainerConstants.USER_PICTURES, blobName, cancellationToken);
                await RemoveOriginalPicture(user.Picture, cancellationToken);
                user.Picture = blobName;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ImageUploadResponse
            {
                Url = MediaLinkConstants.USER_PICTURE + blobName
            };
        }
        
        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageService.RemoveImage(AssetContainerConstants.USER_PICTURES, picture, cancellationToken);
            }
        }
    }
}