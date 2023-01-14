using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.CQRS;
using Audiochan.Core.Dtos;
using Audiochan.Core.Extensions;
using Audiochan.Core.Helpers;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Core.Audios.Commands
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

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService, 
            ISlugGenerator slugGenerator, 
            IUnitOfWork unitOfWork, IDistributedCache cache)
        {
            _currentUserService = currentUserService;
            _slugGenerator = slugGenerator;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<AudioDto>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var currentUserId);

            var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioDto>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<AudioDto>.Forbidden();
            
            UpdateAudioFromCommandAsync(audio, command);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Audio.GetAudio(command.AudioId), cancellationToken);
            
            return Result<AudioDto>.Success(new AudioDto
            {
                Id = audio.Id,
                Description = audio.Description ?? "",
                Src = audio.File,
                IsFavorited = currentUserId > 0
                    ? audio.FavoriteAudios.Any(fa => fa.UserId == currentUserId)
                    : null,
                Slug = HashIdHelper.EncodeLong(audio.Id),
                Created = audio.Created,
                Duration = audio.Duration,
                Picture = audio.Picture,
                Size = audio.Size,
                Tags = audio.Tags,
                Title = audio.Title,
                User = new UserDto
                {
                    Id = audio.UserId,
                    Picture = audio.User.Picture,
                    UserName = audio.User.UserName
                }
            });
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