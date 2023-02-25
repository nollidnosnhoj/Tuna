using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Upload.Common;
using Audiochan.Core.Features.Upload.Models;
using Audiochan.Core.Storage;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Upload;

public enum UploadImageType
{
    Audio,
    User
}

public class CreateImageUploadCommand : CreateUploadCommand
{
    public CreateImageUploadCommand(string fileName, long fileSize, UploadImageType imageType, long userId)
        : base(fileName, fileSize, userId)
    {
        ImageType = imageType;
    }

    public UploadImageType ImageType { get; }
}

public class CreateImageUploadCommandValidator : AbstractValidator<CreateImageUploadCommand>
{
    public CreateImageUploadCommandValidator()
    {
        RuleFor(req => req.FileSize)
            .FileSizeValidation(MediaConfigurationConstants.IMAGE_MAX_FILE_SIZE);
        RuleFor(req => req.FileName)
            .FileNameValidation(MediaConfigurationConstants.IMAGE_VALID_TYPES);
    }
}

public class CreateImageUploadCommandHandler : IRequestHandler<CreateImageUploadCommand, CreateUploadResult>
{
    private readonly IStorageService _storageService;
    private readonly ApplicationSettings _appSettings;

    public CreateImageUploadCommandHandler(
        IStorageService storageService,
        IOptions<ApplicationSettings> appSettings)
    {
        _storageService = storageService;
        _appSettings = appSettings.Value;
    }

    public async Task<CreateUploadResult> Handle(CreateImageUploadCommand request, CancellationToken cancellationToken)
    {
        var (url, uploadId) = await CreateUploadUrl(request.FileName, request.ImageType, request.UserId);
        var response = new CreateUploadResult { UploadId = uploadId, UploadUrl = url };
        return response;
    }
        
    private async Task<(string, string)> CreateUploadUrl(string fileName, UploadImageType type, long userId)
    {
        var fileExt = Path.GetExtension(fileName);
        var uploadId = await Nanoid.Nanoid.GenerateAsync(size: 12);
        var dir = GetSubDir(type);
        var blobName = $"{dir}/{uploadId + fileExt}";
        var metadata = new Dictionary<string, string>
        {
            {"UserId", userId.ToString()},
            {"Type", dir}
        };
        var url = _storageService.CreatePutPreSignedUrl(
            bucket: _appSettings.UploadBucket,
            blobName: blobName,
            expirationInMinutes: 5,
            metadata);
        return (url, uploadId);
    }

    private static string GetSubDir(UploadImageType type)
    {
        return type switch
        {
            UploadImageType.Audio => AssetContainerConstants.AUDIO_PICTURES,
            UploadImageType.User => AssetContainerConstants.USER_PICTURES,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}