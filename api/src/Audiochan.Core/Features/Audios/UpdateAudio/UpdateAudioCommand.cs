using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using FastExpressionCompiler;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioCommand : IRequest<Result<AudioViewModel>>
    {
        public Guid AudioId { get; set; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public Visibility? Visibility { get; init; }
        public List<string>? Tags { get; init; }

        public static UpdateAudioCommand FromRequest(Guid audioId, UpdateAudioRequest request) => new()
        {
            AudioId = audioId,
            Tags = request.Tags,
            Title = request.Title,
            Visibility = request.Visibility,
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

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioViewModel>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _dbContext;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService, 
            ICacheService cacheService, 
            ApplicationDbContext dbContext)
        {
            _currentUserService = currentUserService;
            _cacheService = cacheService;
            _dbContext = dbContext;
        }

        public async Task<Result<AudioViewModel>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var currentUserId))
                return Result<AudioViewModel>.Unauthorized();

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == command.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioViewModel>.NotFound<Audio>();

            if (audio.UserId != currentUserId)
                return Result<AudioViewModel>.Forbidden();
            
            await UpdateAudioFromCommandAsync(audio, command, cancellationToken);
            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(new GetAudioCacheOptions(audio.Id), cancellationToken);
            
            return Result<AudioViewModel>.Success(AudioMaps.AudioToView.CompileFast().Invoke(audio));
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
                    var newTags = await _dbContext.Tags.GetAppropriateTags(command.Tags, cancellationToken);

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
        }
    }
}