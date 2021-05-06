using System.Linq;
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
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequest : AudioAbstractRequest, IRequest<IResult<AudioDetailViewModel>>
    {
        [JsonIgnore] public long AudioId { get; set; }
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
            var currentUserId = await _dbContext.Users
                .Select(u => u.Id)
                .SingleOrDefaultAsync(id => id == _currentUserService.GetUserId(), cancellationToken);

            if (string.IsNullOrEmpty(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == request.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);

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
    }
}