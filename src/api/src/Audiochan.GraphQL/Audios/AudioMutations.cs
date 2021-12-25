using System.Security.Claims;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Features.Audios.Commands.CreateAudio;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.GraphQL.Common.Attributes;
using HotChocolate.AspNetCore.Authorization;
using MediatR;

namespace Audiochan.GraphQL.Audios;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class AudioMutations
{
    [Authorize]
    [UseApplicationDbContext]
    public async Task<Audio> CreateAudio(CreateAudioInput input,
        [Service] IMediator mediator,
        [ScopedService] ApplicationDbContext dbContext,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken = default)
    {
        claimsPrincipal.TryGetUserId(out var userId);
        var command = input.ToCommand(userId);
        var audio = await mediator.Send(command, cancellationToken);
        
        // TODO: Handle result errors

        return audio;
    }
}