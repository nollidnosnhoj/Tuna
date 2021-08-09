using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.CreatePlaylist
{
    public record CreatePlaylistCommand : IRequest<Result<long>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Visibility Visibility { get; set; }
        public List<long> AudioIds { get; set; } = new();
        public List<string> Tags { get; init; } = new();
    }
    
    public class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
    {
        public CreatePlaylistCommandValidator()
        {
            RuleFor(req => req.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(30)
                .WithMessage("Title cannot be no more than 30 characters long.");
            RuleFor(req => req.Description)
                .NotNull()
                .WithMessage("Description cannot be null.")
                .MaximumLength(500)
                .WithMessage("Description cannot be more than 500 characters long.");
            RuleFor(req => req.Tags)
                .NotNull()
                .WithMessage("Tags cannot be null.")
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
        }
    }
    
    public class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, Result<long>>
    {
        private readonly ApplicationDbContext _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISlugGenerator _slugGenerator;

        public CreatePlaylistCommandHandler(ApplicationDbContext unitOfWork, 
            ICurrentUserService currentUserService, ISlugGenerator slugGenerator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _slugGenerator = slugGenerator;
        }

        public async Task<Result<long>> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null) return Result<long>.Unauthorized();

            if (!await CheckIfAudioIdsExist(request.AudioIds, cancellationToken))
            {
                return Result<long>.BadRequest("Audio ids are invalid.");
            }

            var playlist = new Playlist
            {
                Title = request.Title,
                Description = request.Description,
                Visibility = request.Visibility,
                UserId = userId,
                User = user,
                Audios = request.AudioIds
                    .Select(x => new PlaylistAudio
                    {
                        AudioId = x,
                    })
                    .ToList()
            };

            playlist.Slug = _slugGenerator.GenerateSlug(request.Title);

            await _unitOfWork.Playlists.AddAsync(playlist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<long>.Success(playlist.Id);
        }
        
        private async Task<bool> CheckIfAudioIdsExist(ICollection<long> audioIds,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Audios.AnyAsync(x => audioIds.Contains(x.Id)
                && x.Visibility == Visibility.Public, cancellationToken);
        }
    }
}