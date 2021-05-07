using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.CreateAudio;

namespace Audiochan.Core.Common.Builders
{
    public class AudioBuilder
    {
        private readonly Audio _audio;

        public AudioBuilder()
        {
            _audio = new Audio();
        }

        public AudioBuilder GenerateFromCreateRequest(CreateAudioRequest request, string userId)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentNullException(nameof(request.Title));
            
            if (string.IsNullOrWhiteSpace(request.UploadId))
                throw new ArgumentNullException(nameof(request.UploadId));
            
            if (string.IsNullOrWhiteSpace(request.FileName))
                throw new ArgumentNullException(nameof(request.FileName));

            var fileExtension = Path.GetExtension(request.FileName);
            
            if (string.IsNullOrEmpty(fileExtension))
                throw new ArgumentException("File name does not have file extension", nameof(request.FileName));
            
            _audio.UploadId = request.UploadId;
            _audio.FileName = Path.GetFileNameWithoutExtension(request.FileName);
            _audio.FileExt = fileExtension;
            _audio.FileSize = request.FileSize;
            _audio.Duration = request.Duration;
            _audio.UserId = userId;
            _audio.Title = _audio.FileName.Truncate(30);
            
            _audio.UpdateTitle(request.Title);
            _audio.UpdateDescription(request.Description);
            _audio.UpdateVisibility(request.Visibility ?? Visibility.Unlisted);
            
            return this;
        }

        public AudioBuilder AddTitle(string title)
        {
            _audio.UpdateTitle(title);
            return this;
        }

        public AudioBuilder AddDescription(string description)
        {
            _audio.UpdateDescription(description);
            return this;
        }

        public AudioBuilder AddDuration(int duration)
        {
            _audio.Duration = duration;
            return this;
        }

        public AudioBuilder AddUploadId(string uploadId)
        {
            _audio.UploadId = uploadId;
            return this;
        }

        public AudioBuilder AddFileName(string fileName)
        {
            _audio.FileName = fileName;
            _audio.FileExt = Path.GetExtension(fileName);
            return this;
        }

        public AudioBuilder AddFileSize(long size)
        {
            _audio.FileSize = size;
            return this;
        }

        public AudioBuilder AddFileExtension(string fileExtension)
        {
            _audio.FileExt = fileExtension;
            return this;
        }

        public AudioBuilder AddUser(User user)
        {
            _audio.UserId = user.Id;
            _audio.User = user;
            return this;
        }

        public AudioBuilder AddUserId(string userId)
        {
            _audio.UserId = userId;
            return this;
        }

        public AudioBuilder AddTags(List<Tag> tags)
        {
            _audio.Tags = tags;
            return this;
        }

        public AudioBuilder SetVisibility(Visibility visibility)
        {
            _audio.UpdateVisibility(visibility);
            return this;
        }

        public AudioBuilder OverwritePrivateKey(string key)
        {
            _audio.PrivateKey = key;
            return this;
        }

        public async Task<Audio> BuildAsync()
        {
            _audio.Id = await Nanoid.Nanoid.GenerateAsync();
            return _audio;
        }
    }
}