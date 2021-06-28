using System.IO;
using Audiochan.Tests.Common.Builders;
using Bogus;

namespace Audiochan.Tests.Common.Extensions
{
    public static class AudioBuilderExtensions
    {
        public static AudioBuilder UseTestDefaults(this AudioBuilder audioBuilder, 
            string userId,
            string fileName = "test.mp3")
        {
            var random = new Randomizer();

            audioBuilder = audioBuilder
                .AddTitle(Path.GetFileNameWithoutExtension(fileName))
                .AddFileName(fileName)
                .AddDuration(random.Number(30, 300))
                .AddFileSize(random.Number(5000, 25000))
                .AddUserId(userId);

            return audioBuilder;
        }
    }
}