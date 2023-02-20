﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Upload.Common;
using Audiochan.Core.Features.Upload.Dtos;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Upload.Commands.Images;

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

public class CreateImageUploadCommandHandler : IRequestHandler<CreateImageUploadCommand, CreateUploadResponse>
{
    private readonly IRandomIdGenerator _randomIdGenerator;
    private readonly IStorageService _storageService;

    public CreateImageUploadCommandHandler(IRandomIdGenerator randomIdGenerator, IStorageService storageService)
    {
        _randomIdGenerator = randomIdGenerator;
        _storageService = storageService;
    }

    public async Task<CreateUploadResponse> Handle(CreateImageUploadCommand request, CancellationToken cancellationToken)
    {
        var (url, uploadId) = await CreateUploadUrl(request.FileName, request.ImageType, request.UserId);
        var response = new CreateUploadResponse { UploadId = uploadId, UploadUrl = url };
        return response;
    }
        
    private async Task<(string, string)> CreateUploadUrl(string fileName, UploadImageType type, long userId)
    {
        var fileExt = Path.GetExtension(fileName);
        var uploadId = await _randomIdGenerator.GenerateAsync(size: 21);
        var dir = GetSubDir(type);
        var blobName = $"{dir}/{uploadId + fileExt}";
        var metadata = new Dictionary<string, string>
        {
            {"UserId", userId.ToString()},
            {"Type", dir}
        };
        var url = _storageService.CreatePutPreSignedUrl(
            "audiochan",    // TODO: Add bucket to configuration
            blobName,
            5,
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