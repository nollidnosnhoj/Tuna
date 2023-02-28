﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Upload.Models;
using Audiochan.Core.Persistence;
using Audiochan.Core.Storage;
using MediatR;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace Audiochan.Core.Features.Users.Commands;

public class UpdateUserPictureCommand : ICommandRequest<UpdateUserPictureResult>
{
    public long UserId { get; }
    public string? UploadId { get; }

    public UpdateUserPictureCommand(long userId, string? uploadId)
    {
        UserId = userId;
        UploadId = uploadId;
    }
}

[GenerateOneOf]
public partial class UpdateUserPictureResult : OneOfBase<ImageUploadResult, NotFound>
{
    
}

public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, UpdateUserPictureResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly ApplicationSettings _appSettings;

    public UpdateUserPictureCommandHandler(
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        IOptions<ApplicationSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _appSettings = appSettings.Value;
    }

    public async Task<UpdateUserPictureResult> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null) return new NotFound();
        
        if (string.IsNullOrEmpty(command.UploadId))
        {
            await RemoveOriginalPicture(user.ImageId, cancellationToken);
            user.ImageId = null;
        }
        else
        {
            user.ImageId = command.UploadId;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ImageUploadResult.ToUserImage(user.ImageId);
    }
        
    private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(picture))
        {
            var blobName = $"images/users/{picture}";
            await _storageService.RemoveAsync(_appSettings.UploadBucket, blobName, cancellationToken);
        }
    }
}