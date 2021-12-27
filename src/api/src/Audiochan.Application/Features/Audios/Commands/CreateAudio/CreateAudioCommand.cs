using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Commons.Pipelines.Attributes;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Features.Audios.Exceptions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Audios.Commands.CreateAudio
{
    [ExplicitTransaction]
    public record CreateAudioCommand(
        string UploadId,
        string Title,
        string Description,
        string[] Tags,
        string FileName,
        long FileSize,
        decimal Duration) : ICommandRequest<AudioDto>
    {
        public string GetBlobName() => UploadId + Path.GetExtension(FileName);
    }


    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, AudioDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ISlugGenerator _slugGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        private readonly IMapper _mapper;

        public CreateAudioCommandHandler(ISlugGenerator slugGenerator,
            IUnitOfWork unitOfWork, 
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageOptions, 
            ICurrentUserService currentUserService, IMapper mapper)
        {
            _slugGenerator = slugGenerator;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _audioStorageSettings = mediaStorageOptions.Value.Audio;
        }

        public async Task<AudioDto> Handle(CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.User.GetUserId();
            var user = await _unitOfWork.Users.FindAsync(userId, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException();
            }

            if (!await ExistsInTempStorage(command.GetBlobName(), cancellationToken))
            {
                throw new UploadDoesNotExistException(command.UploadId);
            }

            Audio? audio = null;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                audio = new Audio
                {
                    User = user,
                    Size = command.FileSize,
                    Duration = command.Duration,
                    Title = command.Title,
                    Description = command.Description,
                    File = command.GetBlobName(),
                };

                // Create tags
                if (command.Tags.Length > 0)
                {
                    audio.Tags = _slugGenerator.GenerateSlugs(command.Tags).ToArray();
                }

                await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await MoveTempAudioToPublic(audio.File, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return _mapper.Map<AudioDto>(audio);
            }
            catch (Exception)
            {
                // Remove audio that was moved to public bucket
                if (!string.IsNullOrEmpty(audio?.File))
                    await _storageService.RemoveAsync(_audioStorageSettings.Bucket, audio.File, cancellationToken);
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