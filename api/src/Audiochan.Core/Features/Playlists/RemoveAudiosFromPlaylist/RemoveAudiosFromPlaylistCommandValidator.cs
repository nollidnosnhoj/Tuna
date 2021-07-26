using FluentValidation;

namespace Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist
{
    public class RemoveAudiosFromPlaylistCommandValidator : AbstractValidator<RemoveAudiosFromPlaylistCommand>
    {
        public RemoveAudiosFromPlaylistCommandValidator()
        {
            RuleFor(x => x.AudioIds)
                .NotEmpty()
                .WithMessage("Audio ids cannot be empty.");
        }
    }
}