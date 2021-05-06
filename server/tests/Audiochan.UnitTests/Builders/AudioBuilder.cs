using System.Collections.Generic;
using System.IO;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Bogus;

namespace Audiochan.UnitTests.Builders
{
    public class AudioBuilder
    {
        private readonly Audio _audio;
        private readonly Randomizer _randomizer = new();

        public AudioBuilder(string userId, string fileName = "test.mp3")
        {
            _audio = new Audio(
                title: Path.GetFileNameWithoutExtension(fileName),
                uploadId: UploadHelpers.GenerateUploadId(),
                fileName: fileName,
                fileSize: _randomizer.Number(5000, 25000),
                duration: _randomizer.Number(30, 300),
                userId: userId);
        }

        public AudioBuilder Title(string title)
        {
            _audio.Title = title;
            return this;
        }

        public AudioBuilder Description(string description)
        {
            _audio.Description = description;
            return this;
        }

        public AudioBuilder Tags(List<Tag> tags)
        {
            _audio.Tags = tags;
            return this;
        }

        public AudioBuilder SetPublic(bool isPublic, string privateKey = "")
        {
            _audio.UpdatePublicity(isPublic);
            if (!string.IsNullOrWhiteSpace(privateKey))
                _audio.PrivateKey = privateKey;
            return this;
        }

        public Audio Build()
        {
            return _audio;
        }
    }
}