using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using OneOf.Types;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdateUserPictureCommand : ICommandRequest<UpdateUserPictureResult>
{
    public UpdateUserPictureCommand(long userId, string? uploadId)
    {
        UserId = userId;
        UploadId = uploadId;
    }

    public long UserId { get; }
    public string? UploadId { get; }
}

[GenerateOneOf]
public partial class UpdateUserPictureResult : OneOfBase<string, NotFound>
{
}

public class UpdateUserPictureCommandHandler : IRequestHandler<UpdateUserPictureCommand, UpdateUserPictureResult>
{
    private readonly IImageService _imageService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserPictureCommandHandler(IUnitOfWork unitOfWork, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<UpdateUserPictureResult> Handle(UpdateUserPictureCommand command,
        CancellationToken cancellationToken)
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

        return user.ImageId;
    }
}