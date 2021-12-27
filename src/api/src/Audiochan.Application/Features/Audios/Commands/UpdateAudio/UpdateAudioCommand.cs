using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Persistence;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Services;
using Audiochan.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Application.Features.Audios.Commands.UpdateAudio
{
    public record UpdateAudioCommand(
        long AudioId,
        string? Title,
        string? Description,
        string[]? Tags) : ICommandRequest<AudioDto>;

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, AudioDto>
    {
        private readonly IDistributedCache _cache;
        private readonly ISlugGenerator _slugGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateAudioCommandHandler(
            ISlugGenerator slugGenerator, 
            IUnitOfWork unitOfWork,
            IDistributedCache cache, 
            ICurrentUserService currentUserService, IMapper mapper)
        {
            _slugGenerator = slugGenerator;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<AudioDto> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.User.GetUserId();
            
            var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

            if (audio == null)
                throw new NotFoundException<Audio, long>(command.AudioId);

            if (audio.UserId != userId)
                throw new ForbiddenException();
            
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
                audio.Tags = command.Tags.Length == 0 
                    ? Array.Empty<string>() 
                    : _slugGenerator.GenerateSlugs(command.Tags).ToArray();
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