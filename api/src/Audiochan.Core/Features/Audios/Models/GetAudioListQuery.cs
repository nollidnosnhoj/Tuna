using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Features.Audios.Models
{
    public class GetAudioListQuery : PaginationQuery
    {
        public string Username { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string Sort { get; set; } = string.Empty;
    }
}