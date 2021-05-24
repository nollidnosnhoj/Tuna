using System;
using System.Collections.Generic;
using System.IO;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;

namespace Audiochan.UnitTests.Builders
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

        public AudioBuilder AddFileExtension(string fileExt)
        {
            _audio.FileExt = fileExt;
            return this;
        }

        public AudioBuilder AddContentType(string contentType)
        {
            _audio.ContentType = contentType;
            return this;
        }

        public AudioBuilder AddFileSize(long size)
        {
            _audio.FileSize = size;
            return this;
        }

        public AudioBuilder AddUser(User user)
        {
            _audio.User = user;
            _audio.UserId = user.Id;
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

        public Audio Build(string objectId)
        {
            _audio.FileExt = Path.GetExtension(_audio.OriginalFileName);
            _audio.ContentType = _audio.FileName.GetContentType();
            _audio.FileName = objectId + _audio.FileExt;
            
            return _audio;
        }
    }
}