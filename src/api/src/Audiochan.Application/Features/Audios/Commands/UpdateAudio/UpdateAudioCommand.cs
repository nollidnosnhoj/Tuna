using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Entities;
using AutoMapper;
using KopaCore.Result;
using KopaCore.Result.Errors;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Application.Features.Audios.Commands.UpdateAudio
{
    public class UpdateAudioCommand : ICommandRequest<Result<AudioDto>>
    {
        public long AudioId { get; set; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public List<string>? Tags { get; init; }

        public static UpdateAudioCommand FromRequest(long audioId, UpdateAudioRequest request) => new()
        {
            AudioId = audioId,
            Tags = request.Tags,
            Title = request.Title,
            Description = request.Description,
        };
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDto>>
    {
        private readonly IDistributedCache _cache;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISlugGenerator _slugGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService, 
            ISlugGenerator slugGenerator, 
            IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
        {
            _currentUserService = currentUserService;
            _slugGenerator = slugGenerator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<AudioDto>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var currentUserId);

            var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

            if (audio == null)
                return new NotFoundErrorResult<AudioDto>();

            if (audio.UserId != currentUserId)
                return new ForbiddenErrorResult<AudioDto>();
            
            UpdateAudioFromCommandAsync(audio, command);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Audio.GetAudio(command.AudioId), cancellationToken);
            
            return _mapper.Map<AudioDto>(audio);
        }

        private void UpdateAudioFromCommandAsync(Audio audio, UpdateAudioCommand command)
        {
            if (command.Tags is not null)
            {
                if (command.Tags.Count == 0)
                {
                    audio.Tags.Clear();
                }
                else
                {
                    audio.Tags = _slugGenerator.GenerateSlugs(command.Tags).ToList();
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
        }
    }
}