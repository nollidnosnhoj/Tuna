using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Audiochan.Core.Playlists.CreatePlaylist
{
    public record CreatePlaylistCommand : IRequest<Result<long>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
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
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePlaylistCommandHandler(IAuthService authService, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
        {
            var userId = _authService.GetUserId();

            var audios = await GetExistingAudios(request.AudioIds, cancellationToken);
            if (audios.Count == 0)
            {
                return Result<long>.BadRequest("Audio ids are invalid.");
            }

            var playlist = new Playlist
            {
                Title = request.Title,
                Description = request.Description,
                UserId = userId,
                Audios = audios
            };

            await _unitOfWork.Playlists.AddAsync(playlist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<long>.Success(playlist.Id);
        }
        
        private async Task<List<Audio>> GetExistingAudios(ICollection<long> audioIds,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Audios.GetListAsync(x => audioIds.Contains(x.Id), cancellationToken);
        }
    }
}