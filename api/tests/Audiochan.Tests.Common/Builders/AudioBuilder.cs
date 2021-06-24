using System;
using System.Collections.Generic;
using System.IO;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Tests.Common.Builders
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
            _audio.FileName = fileName;
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

        public AudioBuilder SetPublic(Visibility visibility)
        {
            _audio.UpdateVisibility(visibility);
            return this;
        }

        public Audio Build(string objectId)
        {
            _audio.BlobName = objectId + Path.GetExtension(_audio.FileName);
            
            return _audio;
        }
    }
}