using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Audios.Exceptions;
using Audiochan.Core.Features.Audios.Extensions;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Core.Storage;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios;

public class CreateAudioCommand : AuthCommandRequest<long>
{
    public CreateAudioCommand(
        string uploadId,
        string fileName,
        long fileSize,
        decimal duration,
        string title,
        string description,
        ClaimsPrincipal user) : base(user)
    {
        UploadId = uploadId;
        FileName = fileName;
        FileSize = fileSize;
        Duration = duration;
        Title = title;
        Description = description;
        BlobName = UploadId + Path.GetExtension(FileName);
    }
        
    public string UploadId { get; }
    public string FileName { get; }
    public long FileSize { get; }
    public decimal Duration { get; }
    public string Title { get; }
    public string Description { get; }
    public string BlobName { get; }
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
        RuleFor(req => req.FileSize).AudioFileSizeValidation();
        RuleFor(req => req.FileName).AudioFileNameValidation();
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
    
public class CreateAudioCommandHandler : IRequestHandler<CreateAudioCommand, long>
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

    public async Task<long> Handle(CreateAudioCommand command,
        CancellationToken cancellationToken)
    {
        var currentUserId = command.GetUserId();

        if (!await ExistsInTempStorage(command.BlobName, cancellationToken))
        {
            throw new AudioNotUploadedException();
        }

        var audio = new Audio(
            command.Title,
            command.Description,
            command.Duration,
            command.BlobName,
            command.FileSize,
            currentUserId);

        await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return audio.Id;
    }
        
    private async Task<bool> ExistsInTempStorage(string fileName, CancellationToken cancellationToken = default)
    {
        return await _storageService.ExistsAsync(
            _appSettings.UploadBucket,
            fileName,
            cancellationToken);
    }
}