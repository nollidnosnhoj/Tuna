using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Enums;
using Audiochan.Core.Extensions.MappingExtensions;
using Audiochan.Core.Helpers;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.Settings;
using Audiochan.Core.Models.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Features.Audios.CreateAudio
{
    public class CreateAudioRequestHandler : IRequestHandler<CreateAudioRequest, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITagRepository _tagRepository;
        private readonly MediaStorageSettings _storageSettings;

        public CreateAudioRequestHandler(IOptions<MediaStorageSettings> options,
            IApplicationDbContext dbContext,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            ITagRepository tagRepository)
        {
            _storageSettings = options.Value;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _tagRepository = tagRepository;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(CreateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var currentUser = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == _currentUserService.GetUserId(), cancellationToken);

            if (currentUser is null)
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = new Audio(request.Title, request.UploadId, request.FileName, request.FileSize, request.Duration, currentUser);

            audio.UpdateTitle(request.Title);
            audio.UpdateDescription(request.Description);
            audio.UpdatePublicity(request.IsPublic ?? false);

            try
            {
                audio.UpdateTags(request.Tags.Count > 0
                    ? await _tagRepository.GetListAsync(request.Tags, cancellationToken)
                    : new List<Tag>());

                await _dbContext.Audios.AddAsync(audio, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                var viewModel = audio.MapToDetail(_storageSettings);
                
                return Result<AudioDetailViewModel>.Success(viewModel);
            }
            catch (Exception)
            {
                await _storageService.RemoveAsync(_storageSettings.Audio.Container, BlobHelpers.GetAudioBlobName(audio),
                    cancellationToken);
                throw;
            }
        }
    }
}