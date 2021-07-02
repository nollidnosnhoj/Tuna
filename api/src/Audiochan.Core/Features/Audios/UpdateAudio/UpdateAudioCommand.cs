using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Services;
using AutoMapper;
using MediatR;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioCommand : IRequest<Result<AudioDetailViewModel>>
    {
        public long AudioId { get; set; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public Visibility? Visibility { get; init; }
        public List<string>? Tags { get; init; }

        public static UpdateAudioCommand FromRequest(long audioId, UpdateAudioRequest request) => new()
        {
            AudioId = audioId,
            Tags = request.Tags,
            Title = request.Title,
            Visibility = request.Visibility,
            Description = request.Description,
        };
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDetailViewModel>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork,
            ICacheService cacheService, 
            IMapper mapper)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _unitOfWork.Audios
                .LoadForUpdate(command.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (audio.UserId != currentUserId)
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);

            if (command.Tags is not null)
            {
                if (command.Tags.Count == 0)
                {
                    audio.Tags.Clear();
                }
                else
                {
                    var newTags = await _unitOfWork.Tags.GetAppropriateTags(command.Tags, cancellationToken);

                    audio.UpdateTags(newTags);
                }
            }

            if (command.Title is not null && !string.IsNullOrWhiteSpace(command.Title))
            {
                audio.Title = command.Title;
            }

            if (command.Description is not null)
            {
                audio.Description = command.Description;
            }

            if (command.Visibility.HasValue)
            {
                audio.Visibility = command.Visibility.Value;
            }

            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(audio.Id), cancellationToken);
            return Result<AudioDetailViewModel>.Success(_mapper.Map<AudioDetailViewModel>(audio));
        }
    }
}