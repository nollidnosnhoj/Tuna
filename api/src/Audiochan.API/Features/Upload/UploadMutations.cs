using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Errors;
using Audiochan.Shared.Extensions;
using Audiochan.Core.Features.Upload.Commands;
using Audiochan.Core.Features.Upload.Models;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Features.Upload;

[ExtendObjectType(OperationType.Mutation)]
public class UploadMutations
{
    [Authorize]
    [UseValidationError]
    public async Task<CreateUploadResult> CreateAudioUpload(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new CreateAudioUploadCommand(fileName, fileSize, userId);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [UseValidationError]
    public async Task<CreateUploadResult> CreateAudioImageUpload(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new CreateImageUploadCommand(fileName, fileSize, UploadImageType.Audio, userId);
        return await mediator.Send(command, cancellationToken);
    }
    
    [Authorize]
    [UseValidationError]
    public async Task<CreateUploadResult> CreateUserImageUpload(
        string fileName,
        long fileSize,
        IMediator mediator,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var command = new CreateImageUploadCommand(fileName, fileSize, UploadImageType.User, userId);
        return await mediator.Send(command, cancellationToken);
    }
}