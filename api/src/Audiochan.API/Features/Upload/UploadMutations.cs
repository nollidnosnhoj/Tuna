using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Upload;
using Audiochan.Core.Features.Upload.Commands;
using Audiochan.Core.Features.Upload.Models;
using FluentValidation;
using HotChocolate.Authorization;
using HotChocolate.Language;
using HotChocolate.Types;
using MediatR;

namespace Audiochan.API.Features.Upload;

[ExtendObjectType(OperationType.Mutation)]
public class UploadMutations
{
    [Authorize]
    [Error<ValidationException>]
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
    [Error<ValidationException>]
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
    [Error<ValidationException>]
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