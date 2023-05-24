using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Features.Audios.Exceptions;
using Tuna.Application.Features.Audios.Mappings;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Persistence;
using Tuna.Domain.Entities;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Audios.Commands;

public class UpdateAudioCommand : AuthCommandRequest<Result<AudioDto>>
{
    public UpdateAudioCommand(long id, string? title, string? description)
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
    }
}

public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAudioCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AudioDto>> Handle(UpdateAudioCommand command,
        CancellationToken cancellationToken)
    {
        var audio = await _unitOfWork.Audios.FindAsync(command.Id, cancellationToken);

        if (audio == null)
            return new Result<AudioDto>(new AudioNotFoundException(command.Id));
        if (audio.UserId != command.UserId)
            return new Result<AudioDto>(new UnauthorizedAccessException());

        UpdateAudioFromCommandAsync(audio, command);
        _unitOfWork.Audios.Update(audio);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return audio.MapToDto();
    }

    private void UpdateAudioFromCommandAsync(Audio audio, UpdateAudioCommand command)
    {
        if (command.Title is not null && !string.IsNullOrWhiteSpace(command.Title)) audio.Title = command.Title;

        if (command.Description is not null) audio.Description = command.Description;
    }
}