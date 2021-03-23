using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public record CreateAudioCommand : AudioCommandRequest, IRequest<Result<AudioDetailViewModel>>
    {
        public string UploadId { get; init; }
        public string FileName { get; init; }
        public long FileSize { get; init; }
        public int Duration { get; init; }
    }

    public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
    {
        public CreateAudioCommandValidator(IOptions<AudiochanOptions> options)
        {
            var uploadOptions = options.Value.AudioStorageOptions;

            RuleFor(req => req.UploadId)
                .NotEmpty()
                .WithMessage("UploadId is required.");
            RuleFor(req => req.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.");
            RuleFor(req => req.FileSize)
                .NotEmpty()
                .WithMessage("FileSize is required.")
                .LessThanOrEqualTo(uploadOptions.MaxFileSize)
                .WithMessage("FileSize exceeded maximum file size.");
            RuleFor(req => req.FileName)
                .NotEmpty()
                .WithMessage("Filename is required.")
                .Must(Path.HasExtension)
                .WithMessage("Filename must have a file extension.")
                .Must(fileName => uploadOptions.ContentTypes
                    .Contains(Path.GetExtension(fileName).GetContentType()))
                .WithMessage("Filename is invalid.");

            Include(new AudioCommandValidator());
        }
    }

    public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITagRepository _tagRepository;
        private readonly AudiochanOptions.StorageOptions _audioStorageOptions;

        public CreateAudioCommandHandler(IOptions<AudiochanOptions> options,
            IApplicationDbContext dbContext,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            ITagRepository tagRepository)
        {
            _audioStorageOptions = options.Value.AudioStorageOptions;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _tagRepository = tagRepository;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(CreateAudioCommand request,
            CancellationToken cancellationToken)
        {
            var currentUser = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == _currentUserService.GetUserId(), cancellationToken);

            if (currentUser is null)
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = new Audio(request.UploadId, request.FileName, request.FileSize, request.Duration, currentUser.Id);

            audio.UpdateTitle(request.Title);
            audio.UpdateDescription(request.Description);
            audio.UpdatePublicityStatus(request.Publicity);
            
            if (audio.Visibility == Visibility.Private)
                audio.SetPrivateKey();

            try
            {
                audio.UpdateTags(request.Tags.Count > 0
                    ? await _tagRepository.CreateTags(request.Tags, cancellationToken)
                    : new List<Tag>());

                await _dbContext.Audios.AddAsync(audio, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                var viewModel = AudioDetailViewModel.MapFrom(audio) with
                {
                    User = new MetaUserDto(currentUser),
                };
                
                return Result<AudioDetailViewModel>.Success(viewModel);
            }
            catch (Exception)
            {
                await _storageService.RemoveAsync(_audioStorageOptions.Container, BlobHelpers.GetAudioBlobName(audio),
                    cancellationToken);
                throw;
            }
        }
    }
}