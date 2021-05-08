using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class PublishAudioRequest : AudioAbstractRequest, IRequest<Result<AudioDetailViewModel>>
    {
        public string AudioId { get; init; }
    }

    public class PublishAudioRequestHandler : IRequestHandler<PublishAudioRequest, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITagRepository _tagRepository;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PublishAudioRequestHandler(IOptions<MediaStorageSettings> options,
            IApplicationDbContext dbContext,
            IStorageService storageService,
            ICurrentUserService currentUserService,
            ITagRepository tagRepository, 
            IDateTimeProvider dateTimeProvider)
        {
            _storageSettings = options.Value;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _tagRepository = tagRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(PublishAudioRequest request,
            CancellationToken cancellationToken)
        {
            var audio = await _dbContext.Audios
                .Include(u => u.User)
                .SingleOrDefaultAsync(a => a.Id == request.AudioId, cancellationToken);

            if (audio == null || audio.IsPublish)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            var currentUserId = _currentUserService.GetUserId();

            if (audio.UserId != currentUserId)
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);

            try
            {
                audio.UpdateTitle(request.Title);
                audio.UpdateDescription(request.Description);
                audio.UpdatePublicity(request.IsPublic ?? false);
                if (request.Tags.Count > 0)
                    audio.UpdateTags(await _tagRepository.GetListAsync(request.Tags, cancellationToken));
                audio.PublishAudio(_dateTimeProvider.Now);
                _dbContext.Audios.Update(audio);
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