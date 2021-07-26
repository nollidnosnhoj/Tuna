using FluentValidation;

namespace Audiochan.Core.Features.Playlists.AddAudiosToPlaylist
{
    public class AddAudiosToPlaylistCommandValidator : AbstractValidator<AddAudiosToPlaylistCommand>
    {
        public AddAudiosToPlaylistCommandValidator()
        {
            RuleFor(x => x.AudioIds)
                .NotEmpty()
                .WithMessage("Audio ids cannot be empty.");
        }
    }
}