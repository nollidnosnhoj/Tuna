using System.IO;

namespace Audiochan.Core.Common.Models
{
    public record ThumbnailDto(Stream Stream, int Height, int Width){}
}