using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Extensions;
using Audiochan.Common.Interfaces;
using Audiochan.Common.Services;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.UpdatePicture
{
    public class UpdateUserPictureCommand : ICommandRequest<ImageUploadResponse>, IImageData
    {
        public long UserId { get; }
        public string? Data { get; }

        public UpdateUserPictureCommand(long userId, string? data)
        {
            UserId = userId;
            Data = data;
        }
    }

    public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, ImageUploadResponse>
    {
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRandomIdGenerator _randomIdGenerator;

        public UpdateUserPictureCommandHandler(IImageService imageService,
            IUnitOfWork unitOfWork, 
            IRandomIdGenerator randomIdGenerator)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            _randomIdGenerator = randomIdGenerator;
        }

        public async Task<ImageUploadResponse> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException();
            }
        
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