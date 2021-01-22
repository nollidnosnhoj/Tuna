using System.IO;
using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IAudioMetadataService
    {
        AudioMetadataDto GetMetadata(Stream stream, string mimeType);
    }
}