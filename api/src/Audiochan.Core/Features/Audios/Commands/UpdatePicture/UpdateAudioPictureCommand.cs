using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Features.Upload.Dtos;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Audios.Commands.UpdatePicture
{
    public class UpdateAudioPictureCommand : AuthCommandRequest<ImageUploadResponse>
    {
        public UpdateAudioPictureCommand(long audioId, string? uploadId, ClaimsPrincipal user) : base(user)
        {
            AudioId = audioId;
            UploadId = uploadId;
        }

        public long AudioId { get; }
        public string? UploadId { get; }
    }

    public class UpdateAudioPictureCommandHandler : IRequestHandler<UpdateAudioPictureCommand, ImageUploadResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public UpdateAudioPictureCommandHandler(IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task<ImageUploadResponse> Handle(UpdateAudioPictureCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = command.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

            if (audio == null)
                throw new AudioNotFoundException(command.AudioId);

            if (audio.UserId != currentUserId)
                throw new AudioNotFoundException(command.AudioId);

            if (string.IsNullOrEmpty(command.UploadId))
            {
                await RemoveOriginalPicture(audio.ImageId, cancellationToken);
                audio.ImageId = null;
            }
            else
            {
                audio.ImageId = command.UploadId;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ImageUploadResponse.ToAudioImage(audio.ImageId);
        }

        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                var blobName = $"images/audios/{picture}";
                await _storageService.RemoveAsync("audiochan", blobName, cancellationToken);
                // TODO: Add bucket to configuration
            }
        }
    }
}