using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Tuna.Application.Features.Uploads.Models;
using Tuna.Application.Services;
using Tuna.Shared;
using Tuna.Shared.Extensions;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Uploads.Commands;

public enum UploadImageType
{
    Audio,
    User
}

public class CreateImageUploadCommand : ICommandRequest<CreateUploadResult>
{
    public CreateImageUploadCommand(string fileName, long fileSize, UploadImageType imageType, long userId)
    {
        FileName = fileName;
        FileSize = fileSize;
        ImageType = imageType;
        UserId = userId;
    }

    public string FileName { get; }
    public long FileSize { get; }
    public UploadImageType ImageType { get; }
    public long UserId { get; }
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
    private readonly IImageService _imageService;

    public CreateImageUploadCommandHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<CreateUploadResult> Handle(CreateImageUploadCommand request, CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, string>
        {
            { "UserId", request.UserId.ToString() }
        };
        var result = await _imageService.PrepareUploadAsync(metadata);
        var response = new CreateUploadResult(result.UploadId, result.Url);
        return response;
    }
}