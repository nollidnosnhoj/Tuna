using System.Threading;
using System.Threading.Tasks;
using Audiochan.Shared.Mediatr;
using Audiochan.Core.Features.Upload.Models;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
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
    private readonly IImageService _imageService;

    public UpdateUserPictureCommandHandler(IUnitOfWork unitOfWork, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<UpdateUserPictureResult> Handle(UpdateUserPictureCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null) return new NotFound();
        
        if (string.IsNullOrEmpty(command.UploadId) && !string.IsNullOrEmpty(user.ImageId))
        {
            await _imageService.DeleteImageAsync(user.ImageId);
            user.ImageId = null;
        }
        else
        {
            user.ImageId = command.UploadId;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ImageUploadResult.ToUserImage(user.ImageId);
    }
}