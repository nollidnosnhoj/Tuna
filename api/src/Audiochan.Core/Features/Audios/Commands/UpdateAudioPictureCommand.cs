using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Features.Upload.Models;
using Audiochan.Core.Persistence;
using Audiochan.Core.Storage;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.Commands
{
    public class UpdateAudioPictureCommand : AuthCommandRequest<ImageUploadResult>
    {
        public UpdateAudioPictureCommand(long audioId, string? uploadId, ClaimsPrincipal user) : base(user)
        {
            AudioId = audioId;
            UploadId = uploadId;
        }

        public long AudioId { get; }
        public string? UploadId { get; }
    }

    public class UpdateAudioPictureCommandHandler : IRequestHandler<UpdateAudioPictureCommand, ImageUploadResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly ApplicationSettings _appSettings;

        public UpdateAudioPictureCommandHandler(
            IUnitOfWork unitOfWork, 
            IStorageService storageService, 
            IOptions<ApplicationSettings> applicationSettings)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _appSettings = applicationSettings.Value;
        }

        public async Task<ImageUploadResult> Handle(UpdateAudioPictureCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = command.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

            if (audio == null)
                throw new ResourceIdInvalidException<long>(typeof(Audio), command.AudioId);

            if (audio.UserId != currentUserId)
                throw new ResourceOwnershipException<long>(typeof(Audio), command.AudioId, currentUserId);

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

            return ImageUploadResult.ToAudioImage(audio.ImageId);
        }

        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                var blobName = $"images/audios/{picture}";
                await _storageService.RemoveAsync(_appSettings.UploadBucket, blobName, cancellationToken);
            }
        }
    }
}