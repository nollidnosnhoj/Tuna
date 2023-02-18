using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Extensions;
using Audiochan.Common.Helpers;
using Audiochan.Core.Features.Audios.Dtos;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Core.Features.Audios.Commands.UpdateAudio
{
    public class UpdateAudioCommand : AuthCommandRequest<AudioDto>
    {
        public UpdateAudioCommand(long id, string? title, string? description, ClaimsPrincipal user) : base(user)
        {
            Id = id;
            Title = title;
            Description = description;
        }
        
        public long Id { get; }
        public string? Title { get; }
        public string? Description { get; }
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, AudioDto>
    {
        private readonly IDistributedCache _cache;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAudioCommandHandler(
            IUnitOfWork unitOfWork, 
            IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<AudioDto> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = command.GetUserId();

            var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

            if (audio == null)
                throw new AudioNotFoundException(command.Id);

            if (audio.UserId != currentUserId)
                throw new AudioNotFoundException(command.Id);
            
            UpdateAudioFromCommandAsync(audio, command);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Audio.GetAudio(command.Id), cancellationToken);

            return new AudioDto
            {
                Id = audio.Id,
                Description = audio.Description ?? "",
                ObjectKey = audio.ObjectKey,
                Created = audio.Created,
                Duration = audio.Duration,
                Picture = audio.Picture,
                Size = audio.Size,
                Title = audio.Title,
                User = new UserDto
                {
                    Id = audio.UserId,
                    Picture = audio.User.Picture,
                    UserName = audio.User.UserName
                }
            };
        }

        private void UpdateAudioFromCommandAsync(Audio audio, UpdateAudioCommand command)
        {
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