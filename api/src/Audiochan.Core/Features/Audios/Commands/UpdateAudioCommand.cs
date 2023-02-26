using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Core.Features.Audios.Commands;

public class UpdateAudioCommand : AuthCommandRequest<AudioViewModel>
{
    public UpdateAudioCommand(long id, string? title, string? description, ClaimsPrincipal user) : base(user)
    {
        Id = id;
        Title = title;
        Description = description;
    }
        
    public long Id { get; }
    public string? Title { get; }
    public string? Description { get; }
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

        // When(req => req.Tags is not null, () =>
        // {
        //     RuleFor(req => req.Tags)
        //         .Must(u => u!.Count <= 10)
        //         .WithMessage("Can only have up to 10 tags per audio upload.")
        //         .ForEach(tagsRule =>
        //         {
        //             tagsRule
        //                 .NotEmpty()
        //                 .WithMessage("Each tag cannot be empty.")
        //                 .Length(3, 15)
        //                 .WithMessage("Each tag must be between 3 and 15 characters long.");
        //         });
        // });
    }
}

public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, AudioViewModel>
{
    private readonly IDistributedCache _cache;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAudioCommandHandler(
        IUnitOfWork unitOfWork, 
        IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<AudioViewModel> Handle(UpdateAudioCommand command,
        CancellationToken cancellationToken)
    {
        var currentUserId = command.GetUserId();

        var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

        if (audio == null)
            throw new ResourceIdInvalidException<long>(typeof(Audio), command.Id);

        if (audio.UserId != currentUserId)
            throw new ResourceOwnershipException<long>(typeof(Audio), command.Id, currentUserId);
            
        UpdateAudioFromCommandAsync(audio, command);
        _unitOfWork.Audios.Update(audio);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AudioViewModel
        {
            Id = audio.Id,
            Description = audio.Description ?? "",
            ObjectKey = audio.ObjectKey,
            Created = audio.CreatedAt,
            Duration = audio.Duration,
            Picture = audio.ImageId,
            Size = audio.Size,
            Title = audio.Title,
            User = new UserViewModel
            {
                Id = audio.UserId,
                Picture = audio.User.ImageId,
                UserName = audio.User.UserName
            }
        };
    }

    private void UpdateAudioFromCommandAsync(Audio audio, UpdateAudioCommand command)
    {
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