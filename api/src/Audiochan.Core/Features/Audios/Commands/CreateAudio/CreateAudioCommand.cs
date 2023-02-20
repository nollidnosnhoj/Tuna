using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Pipelines.Attributes;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.Commands.CreateAudio
{
    [ExplicitTransaction]
    public class CreateAudioCommand : AuthCommandRequest<long>
    {
        public CreateAudioCommand(
            string uploadId,
            string fileName,
            long fileSize,
            decimal duration,
            string title,
            string description,
            ClaimsPrincipal user) : base(user)
        {
            UploadId = uploadId;
            FileName = fileName;
            FileSize = fileSize;
            Duration = duration;
            Title = title;
            Description = description;
            BlobName = UploadId + Path.GetExtension(FileName);
        }
        
        public string UploadId { get; }
        public string FileName { get; }
        public long FileSize { get; }
        public decimal Duration { get; }
        public string Title { get; }
        public string Description { get; }
        public string BlobName { get; }
    }


    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;

        public CreateAudioCommandHandler(IUnitOfWork unitOfWork,
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageOptions)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageOptions.Value.Audio;
        }

        public async Task<long> Handle(CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = command.GetUserId();

            if (!await ExistsInTempStorage(command.BlobName, cancellationToken))
            {
                throw new AudioNotUploadedException();
            }

            Audio? audio = null;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                audio = new Audio
                {
                    UserId = currentUserId,
                    Size = command.FileSize,
                    Duration = command.Duration,
                    Title = command.Title,
                    Description = command.Description,
                    ObjectKey = command.BlobName,
                };

                await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await MoveTempAudioToPublic(audio.ObjectKey, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return audio.Id;
            }
            catch (Exception)
            {
                // Remove audio that was moved to public bucket
                if (!string.IsNullOrEmpty(audio?.ObjectKey))
                    await _storageService.RemoveAsync(_audioStorageSettings.Bucket, audio.ObjectKey, cancellationToken);
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        
        private async Task MoveTempAudioToPublic(string fileName, CancellationToken cancellationToken = default)
        {
            await _storageService.MoveBlobAsync(
                _audioStorageSettings.TempBucket,
                fileName,
                _audioStorageSettings.Bucket,
                $"audios/{fileName}",
                cancellationToken: cancellationToken);
        }
        
        private async Task<bool> ExistsInTempStorage(string fileName, CancellationToken cancellationToken = default)
        {
            return await _storageService.ExistsAsync(
                _audioStorageSettings.TempBucket,
                fileName,
                cancellationToken);
        }
    }
}