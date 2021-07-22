using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using FluentValidation;

namespace Audiochan.Core.Features.Playlists.CreatePlaylist
{
    public class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public CreatePlaylistCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        }
    }
}