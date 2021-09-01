using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using FastExpressionCompiler;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.UpdateAudio
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

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ISlugGenerator _slugGenerator;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService, 
            ICacheService cacheService, 
            ApplicationDbContext dbContext, ISlugGenerator slugGenerator)
        {
            _currentUserService = currentUserService;
            _cacheService = cacheService;
            _dbContext = dbContext;
            _slugGenerator = slugGenerator;
        }

        public async Task<Result<AudioDto>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var currentUserId))
                return Result<AudioDto>.Unauthorized();

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == command.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioDto>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<AudioDto>.Forbidden();
            
            await UpdateAudioFromCommandAsync(audio, command, cancellationToken);
            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(audio.Id), cancellationToken);
            
            return Result<AudioDto>.Success(AudioMaps.AudioToView().CompileFast().Invoke(audio));
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
                    var newTags = await _dbContext.Tags.GetAppropriateTags(tagStrings, cancellationToken);
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