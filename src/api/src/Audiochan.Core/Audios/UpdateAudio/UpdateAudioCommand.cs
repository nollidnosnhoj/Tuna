using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Audios.GetAudio;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Audios.UpdateAudio
{
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

    public sealed class LoadAudioForUpdateSpecification : Specification<Audio>
    {
        public LoadAudioForUpdateSpecification(long audioId)
        {
            Query.Include(a => a.User);
            Query.Include(a => a.Tags);
            Query.Where(a => a.Id == audioId);
        }
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDto>>
    {
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;
        private readonly ISlugGenerator _slugGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAudioCommandHandler(IAuthService authService, 
            ICacheService cacheService, 
            ISlugGenerator slugGenerator, 
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _authService = authService;
            _cacheService = cacheService;
            _slugGenerator = slugGenerator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<AudioDto>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = _authService.GetUserId();

            var audio = await _unitOfWork.Audios
                .GetFirstAsync(new LoadAudioForUpdateSpecification(command.AudioId), cancellationToken);

            if (audio == null)
                return Result<AudioDto>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<AudioDto>.Forbidden();
            
            await UpdateAudioFromCommandAsync(audio, command, cancellationToken);
            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(audio.Id), cancellationToken);
            
            return Result<AudioDto>.Success(_mapper.Map<AudioDto>(audio));
        }

        private async Task UpdateAudioFromCommandAsync(Audio audio, UpdateAudioCommand command,
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
                    var tagStrings = _slugGenerator.GenerateSlugs(command.Tags);
                    var newTags = await _unitOfWork.Tags.GetAppropriateTags(tagStrings, cancellationToken);
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
        }
    }
}