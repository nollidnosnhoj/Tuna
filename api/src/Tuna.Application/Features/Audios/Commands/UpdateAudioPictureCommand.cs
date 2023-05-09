using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using OneOf.Types;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Errors;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Audios.Commands;

public class UpdateAudioPictureCommand : AuthCommandRequest<UpdateAudioPictureResult>
{
    public UpdateAudioPictureCommand(long audioId, string? uploadId)
    {
        AudioId = audioId;
        UploadId = uploadId;
    }

    public long AudioId { get; }
    public string? UploadId { get; }
}

[GenerateOneOf]
public partial class UpdateAudioPictureResult : OneOfBase<string, NotFound, Forbidden>
{
}

public class UpdateAudioPictureCommandHandler : IRequestHandler<UpdateAudioPictureCommand, UpdateAudioPictureResult>
{
    private readonly IImageService _imageService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAudioPictureCommandHandler(IUnitOfWork unitOfWork, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<UpdateAudioPictureResult> Handle(UpdateAudioPictureCommand command,
        CancellationToken cancellationToken)
    {
        var audio = await _unitOfWork.Audios.FindAsync(command.AudioId, cancellationToken);

        if (audio == null) return new NotFound();

        if (audio.UserId != command.UserId) return new Forbidden();

        if (string.IsNullOrEmpty(command.UploadId) && !string.IsNullOrEmpty(audio.ImageId))
        {
            await _imageService.DeleteImageAsync(audio.ImageId);
            audio.ImageId = null;
        }
        else
        {
            audio.ImageId = command.UploadId;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return audio.ImageId;
    }
}