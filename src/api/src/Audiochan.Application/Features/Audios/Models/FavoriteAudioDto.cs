using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Audios.Models;

public class FavoriteAudioDto : IMapFrom<FavoriteAudio>
{
    public long AudioId { get; set; }
    public AudioDto Audio { get; set; } = null!;
    public long UserId { get; set; }
    public UserDto User { get; set; } = null!;
}