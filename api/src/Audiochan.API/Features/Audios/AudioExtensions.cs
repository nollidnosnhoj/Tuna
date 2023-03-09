using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Users.DataLoaders;
using Audiochan.Core.Features.Users.Models;
using HashidsNet;
using HotChocolate;
using HotChocolate.Types;

namespace Audiochan.API.Features.Audios;

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

    [BindMember(nameof(AudioDto.Picture))]
    public string? GetImageUrl([Parent] AudioDto audio)
    {
        if (audio.Picture is null) return null;
        return MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
    }
}