using System.Threading;
using System.Threading.Tasks;
using Tuna.Application;
using Tuna.Application.Features.Audios.Models;
using Tuna.Application.Features.Users.DataLoaders;
using Tuna.Application.Features.Users.Models;
using Tuna.Shared;
using HashidsNet;
using HotChocolate;
using HotChocolate.Types;
using Tuna.Application.Services;

namespace Tuna.GraphQl.Features.Audios;

[ExtendObjectType(typeof(AudioDto))]
public sealed class AudioExtensions
{
    [BindMember(nameof(AudioDto.UserId))]
    public async Task<UserDto> GetUserAsync([Parent] AudioDto audio, GetUserDataLoader dataloader,
        CancellationToken cancellationToken)
    {
        return await dataloader.LoadAsync(audio.UserId, cancellationToken);
    }

    public string GetSlug([Parent] AudioDto audio, IHashids hashids)
    {
        return hashids.EncodeLong(audio.Id);
    }

    [BindMember(nameof(AudioDto.ObjectKey))]
    public string GetStreamUrl([Parent] AudioDto audio)
    {
        return MediaLinkConstants.AUDIO_STREAM + audio.ObjectKey;
    }

    [BindMember(nameof(AudioDto.ImageId))]
    public string? GetImageUrl([Parent] AudioDto audio, [Service] IImageService imageService)
    {
        if (audio.ImageId is null) return null;
        return imageService.GetImageUrl(audio.ImageId, null);
    }
}