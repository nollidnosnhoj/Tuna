using System.Threading;
using System.Threading.Tasks;
using HashidsNet;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Tuna.Application;
using Tuna.Application.Features.Audios.DataLoaders;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Features.Users.DataLoaders;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Services;
using Tuna.Shared;

namespace Tuna.GraphQl.Features.Audios;

[Node]
[ExtendObjectType(typeof(AudioDto))]
public static class AudioNode
{
    [NodeResolver]
    public static Task<AudioDto> GetAudioAsync(long id, GetAudioDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        return dataLoader.LoadAsync(id, cancellationToken);
    }
    
    [BindMember(nameof(AudioDto.UserId))]
    public static async Task<UserDto> GetUserAsync([Parent] AudioDto audio, GetUserDataLoader dataloader,
        CancellationToken cancellationToken)
    {
        return await dataloader.LoadAsync(audio.UserId, cancellationToken);
    }

    public static string GetSlug([Parent] AudioDto audio, IHashids hashids)
    {
        return hashids.EncodeLong(audio.Id);
    }

    [BindMember(nameof(AudioDto.ObjectKey))]
    public static string GetStreamUrl([Parent] AudioDto audio)
    {
        return MediaLinkConstants.AUDIO_STREAM + audio.ObjectKey;
    }

    [BindMember(nameof(AudioDto.ImageId))]
    public static string? GetImageUrl([Parent] AudioDto audio, [Service] IImageService imageService)
    {
        if (audio.ImageId is null) return null;
        return imageService.GetImageUrl(audio.ImageId, null);
    }
}