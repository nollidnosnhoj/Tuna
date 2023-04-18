using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Shared;
using Audiochan.Shared.Extensions;
using Audiochan.Shared.Mediatr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.Commands;

public record CreateAudioResult(long Id, string UploadUrl);

public class CreateAudioCommand : ICommandRequest<CreateAudioResult>
{
    public CreateAudioCommand(string fileName, long fileSize, long userId)
    {
        FileName = fileName;
        FileSize = fileSize;
        UserId = userId;
    }

    public string FileName { get; set; }
    public long FileSize { get; set; }
    public long UserId { get; set; }
}
    
public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
{
    public CreateAudioCommandValidator()
    {
        RuleFor(req => req.FileSize)
            .FileSizeValidation(MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE);
        RuleFor(req => req.FileName)
            .FileNameValidation(MediaConfigurationConstants.AUDIO_VALID_TYPES);
    }
}

public class CreateAudioCommandHandler 
    : IRequestHandler<CreateAudioCommand, CreateAudioResult>
{
    private readonly IStorageService _storageService;
    private readonly ApplicationSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAudioCommandHandler(
        IStorageService storageService, 
        IOptions<ApplicationSettings> appSettings, 
        IUnitOfWork unitOfWork)
    {
        _storageService = storageService;
        _unitOfWork = unitOfWork;
        _appSettings = appSettings.Value;
    }
        
    public async Task<CreateAudioResult> Handle(CreateAudioCommand command, 
        CancellationToken cancellationToken)
    {
        var fileId = await Nanoid.Nanoid.GenerateAsync(size: 12);
        
        var audio = new Audio(fileId, command.FileName, command.FileSize, command.UserId);
        await _unitOfWork.Audios.AddAsync(audio, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var blobName = $"{AssetContainerConstants.AUDIO_STREAM}/{audio.Id}/${fileId}.mp3";
        var metadata = new Dictionary<string, string> {{"UserId", command.UserId.ToString()}};
        var url = _storageService.CreatePutPreSignedUrl(
            bucket: _appSettings.UploadBucket,
            blobName: blobName,
            expirationInMinutes: 5,
            metadata);
        return new CreateAudioResult(audio.Id, url);
    }
}