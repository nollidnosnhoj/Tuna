using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Shared.Extensions;
using Audiochan.Shared.Mediatr;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Audios.Results;
using Audiochan.Core.Persistence;
using Audiochan.Core.Storage;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using OneOf;

namespace Audiochan.Core.Features.Audios.Commands;

public class CreateAudioCommand : ICommandRequest<CreateAudioResult>
{
    public CreateAudioCommand(
        string uploadId,
        string fileName,
        long fileSize,
        decimal duration,
        string title,
        string description, long userId)
    {
        UploadId = uploadId;
        FileName = fileName;
        FileSize = fileSize;
        Duration = duration;
        Title = title;
        Description = description;
        UserId = userId;
        BlobName = UploadId + Path.GetExtension(FileName);
    }
        
    public string UploadId { get; }
    public string FileName { get; }
    public long FileSize { get; }
    public decimal Duration { get; }
    public string Title { get; }
    public string Description { get; }
    public long UserId { get; }
    public string BlobName { get; }
}

[GenerateOneOf]
public partial class CreateAudioResult : OneOfBase<AudioDto, AudioNotUploaded>
{
    
}
    
public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
{
    public CreateAudioCommandValidator()
    {
        RuleFor(req => req.UploadId)
            .NotEmpty()
            .WithMessage("UploadId is required.");
        RuleFor(req => req.Duration)
            .NotEmpty()
            .WithMessage("Duration is required.");
        RuleFor(req => req.FileSize)
            .FileSizeValidation(MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE);
        RuleFor(req => req.FileName)
            .FileNameValidation(MediaConfigurationConstants.AUDIO_VALID_TYPES);
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
        // RuleFor(req => req.Tags)
        //     .NotNull()
        //     .WithMessage("Tags cannot be null.")
        //     .Must(u => u!.Count <= 10)
        //     .WithMessage("Can only have up to 10 tags per audio upload.")
        //     .ForEach(tagsRule =>
        //     {
        //         tagsRule
        //             .NotEmpty()
        //             .WithMessage("Each tag cannot be empty.")
        //             .Length(3, 15)
        //             .WithMessage("Each tag must be between 3 and 15 characters long.");
        //     });
    }
}
    
public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, CreateAudioResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly ApplicationSettings _appSettings;

    public CreateAudioCommandHandler(IUnitOfWork unitOfWork, IStorageService storageService, IOptions<ApplicationSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _appSettings = appSettings.Value;
    }

    public async Task<CreateAudioResult> Handle(CreateAudioCommand command,
        CancellationToken cancellationToken)
    {
        if (!await ExistsInTempStorage(command.BlobName, cancellationToken))
        {
            return new AudioNotUploaded(command.UploadId);
        }

        var audio = new Audio(
            command.Title,
            command.Description,
            command.Duration,
            command.BlobName,
            command.FileSize,
            command.UserId);

        await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AudioDto
        {
            Id = audio.Id,
            Description = audio.Description ?? "",
            ObjectKey = audio.ObjectKey,
            Created = audio.CreatedAt,
            Duration = audio.Duration,
            Picture = audio.ImageId,
            Size = audio.Size,
            Title = audio.Title
        };
    }
        
    private async Task<bool> ExistsInTempStorage(string fileName, CancellationToken cancellationToken = default)
    {
        return await _storageService.ExistsAsync(
            _appSettings.UploadBucket,
            fileName,
            cancellationToken);
    }
}