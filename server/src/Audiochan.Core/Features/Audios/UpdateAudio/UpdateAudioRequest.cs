using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequest : AudioAbstractRequest, IRequest<IResult<AudioDetailViewModel>>
    {
        [JsonIgnore] public Guid AudioId { get; set; }
    }


    public class UpdateAudioRequestHandler : IRequestHandler<UpdateAudioRequest, IResult<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITagRepository _tagRepository;
        private readonly MediaStorageSettings _storageSettings;

        public UpdateAudioRequestHandler(IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            ITagRepository tagRepository,
            IOptions<MediaStorageSettings> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _tagRepository = tagRepository;
            _storageSettings = options.Value;
        }

        public async Task<IResult<AudioDetailViewModel>> Handle(UpdateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var (audio, errorResult) = await GetAudioAsync(request.AudioId, cancellationToken);

            if (audio == null)
                return errorResult!;

            if (request.Tags.Count > 0)
            {
                var newTags = await _tagRepository.GetListAsync(request.Tags, cancellationToken);

                audio.UpdateTags(newTags);
            }

            audio.UpdateTitle(request.Title);
            audio.UpdateDescription(request.Description);

            if (request.IsPublic.HasValue)
                audio.UpdatePublicity(request.IsPublic.GetValueOrDefault());

            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var viewModel = audio.MapToDetail(_storageSettings);

            return Result<AudioDetailViewModel>.Success(viewModel);
        }

        public async Task<(Audio?, Result<AudioDetailViewModel>?)> GetAudioAsync(Guid audioId,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);

            if (audio == null)
                return (null, Result<AudioDetailViewModel>.Fail(ResultError.NotFound));

            if (!audio.CanModify(currentUserId))
                return (null, Result<AudioDetailViewModel>.Fail(ResultError.NotFound));

            return (audio, null);
        } 
    }
}