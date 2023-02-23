using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Audios.Extensions;
using Audiochan.Core.Features.Upload.Common;
using Audiochan.Core.Features.Upload.Dtos;
using Audiochan.Core.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Upload.Commands;

public class CreateAudioUploadCommand : CreateUploadCommand
{
    public CreateAudioUploadCommand(string fileName, long fileSize, long userId)
        : base(fileName, fileSize, userId)
    {
    }
}
    
public class CreateAudioUploadCommandValidator : AbstractValidator<CreateAudioUploadCommand>
{
    public CreateAudioUploadCommandValidator()
    {
        RuleFor(req => req.FileSize).AudioFileSizeValidation();
        RuleFor(req => req.FileName).AudioFileNameValidation();
    }
}

public class CreateAudioUploadCommandHandler 
    : IRequestHandler<CreateAudioUploadCommand, CreateUploadResponse>
{
    private readonly IRandomIdGenerator _randomIdGenerator;
    private readonly IStorageService _storageService;
    private readonly ApplicationSettings _appSettings;

    public CreateAudioUploadCommandHandler(
        IRandomIdGenerator randomIdGenerator, 
        IStorageService storageService, 
        IOptions<ApplicationSettings> appSettings)
    {
        _randomIdGenerator = randomIdGenerator;
        _storageService = storageService;
        _appSettings = appSettings.Value;
    }
        
    public async Task<CreateUploadResponse> Handle(CreateAudioUploadCommand command, 
        CancellationToken cancellationToken)
    {
        var (url, uploadId) = await CreateUploadUrl(command.FileName, command.UserId);
        var response = new CreateUploadResponse { UploadId = uploadId, UploadUrl = url };
        return response;
    }
        
    private async Task<(string, string)> CreateUploadUrl(string fileName, long userId)
    {
        var fileExt = Path.GetExtension(fileName);
        var uploadId = await _randomIdGenerator.GenerateAsync(size: 21);
        var blobName = $"{AssetContainerConstants.AUDIO_STREAM}/{uploadId}/${uploadId + fileExt}";
        var metadata = new Dictionary<string, string> {{"UserId", userId.ToString()}};
        var url = _storageService.CreatePutPreSignedUrl(
            bucket: _appSettings.UploadBucket,
            blobName: blobName,
            expirationInMinutes: 5,
            metadata);
        return (url, uploadId);
    }
}