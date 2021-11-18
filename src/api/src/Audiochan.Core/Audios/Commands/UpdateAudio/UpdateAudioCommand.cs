using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Core.Audios.Commands
{
    [Authorize]
    public class UpdateAudioCommand : IRequest<Result<AudioDto>>
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
    
    public class UpdateAudioCommandValidator : AbstractValidator<UpdateAudioCommand>
    {
        public UpdateAudioCommandValidator()
        {
            When(req => req.Title is not null, () =>
            {
                RuleFor(req => req.Title)
                    .NotEmpty()
                    .WithMessage("Title is required.")
                    .MaximumLength(30)
                    .WithMessage("Title cannot be no more than 30 characters long.");
            });

            When(req => req.Description is not null, () =>
            {
                RuleFor(req => req.Description)
                    .MaximumLength(500)
                    .WithMessage("Description cannot be more than 500 characters long.");
            });

            When(req => req.Tags is not null, () =>
            {
                RuleFor(req => req.Tags)
                    .Must(u => u!.Count <= 10)
                    .WithMessage("Can only have up to 10 tags per audio upload.")
                    .ForEach(tagsRule =>
                    {
                        tagsRule
                            .NotEmpty()
                            .WithMessage("Each tag cannot be empty.")
                            .Length(3, 15)
                            .WithMessage("Each tag must be between 3 and 15 characters long.");
                    });
            });
        }
    }
    
    public sealed class LoadAudioWithOwnerSpecification : Specification<Audio>
    {
        public LoadAudioWithOwnerSpecification(long audioId)
        {
            Query.Include(a => a.Artist);
            Query.Where(a => a.Id == audioId);
        }
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

            var audio = await _unitOfWork.Audios
                .GetFirstAsync(new LoadAudioWithOwnerSpecification(command.AudioId), cancellationToken);

            if (audio == null)
                return Result<AudioDto>.NotFound<Audio>();

            if (audio.ArtistId != currentUserId)
                return Result<AudioDto>.Forbidden();
            
            UpdateAudioFromCommandAsync(audio, command, cancellationToken);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Audio.GetAudio(command.AudioId), cancellationToken);
            
            return Result<AudioDto>.Success(_mapper.Map<AudioDto>(audio));
        }

        private void UpdateAudioFromCommandAsync(Audio audio, UpdateAudioCommand command,
            CancellationToken cancellationToken)
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