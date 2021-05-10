using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Audiochan.Core.Common.Exceptions;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Common.Builders
{
    public class AudioBuilder
    {
        private readonly Audio _audio;

        public AudioBuilder(DateTime createdDate) : this()
        {
            _audio.Created = createdDate;
        }
        
        public AudioBuilder()
        {
            _audio = new Audio();
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

        public AudioBuilder AddDuration(decimal duration)
        {
            _audio.Duration = duration;
            return this;
        }

        public AudioBuilder AddFileName(string fileName)
        {
            _audio.OriginalFileName = fileName;
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

        public AudioBuilder SetPublic(bool isPublic)
        {
            _audio.UpdatePublicity(isPublic);
            return this;
        }

        public AudioBuilder SetPublish(DateTime publishDate)
        {
            _audio.PublishAudio(publishDate);
            return this;
        }

        public AudioBuilder SetUnPublish()
        {
            _audio.UnPublishAudio();
            return this;
        }

        public async Task<Audio> BuildAsync()
        {
            if (string.IsNullOrWhiteSpace(_audio.Title))
                throw new BuilderException("Cannot build Audio. Requires title.");

            if (string.IsNullOrWhiteSpace(_audio.FileExt))
                throw new BuilderException("Cannot build Audio. Requires file extension.");

            if (string.IsNullOrWhiteSpace(_audio.OriginalFileName))
                throw new BuilderException("Cannot build Audio. Requires original file name.");

            if (_audio.FileSize <= 0)
                throw new BuilderException("Cannot build Audio. Requires a positive file size.");

            if (_audio.Duration <= 0)
                throw new BuilderException("Cannot build Audio. Requires a positive duration.");

            if (string.IsNullOrWhiteSpace(_audio.UserId))
                throw new BuilderException("Cannot build Audio. Requires a user id.");
            
            _audio.Id = await Nanoid.Nanoid.GenerateAsync();
            _audio.FileName = _audio.Id + _audio.FileExt;
            
            return _audio;
        }
    }
}