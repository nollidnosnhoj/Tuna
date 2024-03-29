﻿using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Features.Audios.Exceptions;
using Tuna.Application.Features.Audios.Mappings;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Persistence;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Audios.Commands;

public class PublishAudioCommand : ICommandRequest<Result<AudioDto>>
{
    public PublishAudioCommand(
        long audioId,
        string title,
        string description,
        decimal duration,
        long userId)
    {
        AudioId = audioId;
        Duration = duration;
        Title = title;
        Description = description;
        UserId = userId;
    }

    public long AudioId { get; }
    public decimal Duration { get; }
    public string Title { get; }
    public string Description { get; }
    public long UserId { get; }
}

public class PublishAudioCommandValidator : AbstractValidator<PublishAudioCommand>
{
    public PublishAudioCommandValidator()
    {
        RuleFor(req => req.AudioId)
            .NotEmpty()
            .WithMessage("AudioId is required.");
        RuleFor(req => req.Duration)
            .NotEmpty()
            .WithMessage("Duration is required.");
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

public class PublishAudioCommandHandler : IRequestHandler<PublishAudioCommand, Result<AudioDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PublishAudioCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AudioDto>> Handle(PublishAudioCommand command,
        CancellationToken cancellationToken)
    {
        var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);
        if (audio is null || audio.UserId != command.UserId)
            return new Result<AudioDto>(new AudioNotFoundException(command.AudioId));
        if (audio.UploadedAt is null)
            return new Result<AudioDto>(new AudioNotUploadedException(command.AudioId));

        audio.Title = command.Title;
        audio.Description = command.Description;
        audio.Duration = command.Duration;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return audio.MapToDto();
    }
}