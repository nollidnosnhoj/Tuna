using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audiochan.Core.Entities.Base;
using Audiochan.Core.Enums;
using Audiochan.Core.Extensions;

namespace Audiochan.Core.Entities
{
    public class Audio : BaseEntity
    {
        public Audio()
        {
            this.Tags = new HashSet<Tag>();
        }

        public Audio(string uploadId, string fileName, long fileSize, int duration, User user)
            : this(uploadId, fileName, fileSize, duration, user.Id)
        {
            this.User = user;
        }

        public Audio(string uploadId, string fileName, long fileSize, int duration, string userId) : this()
        {
            if (string.IsNullOrWhiteSpace(uploadId))
                throw new ArgumentException("UploadId cannot be empty.", nameof(uploadId));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            var fileExtension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(fileExtension))
                throw new ArgumentException("File name does not have file extension", nameof(fileName));

            this.UploadId = uploadId;
            this.FileExt = fileExtension;
            this.FileSize = fileSize;
            this.Duration = duration;
            this.UserId = userId;

            this.Title = Path.GetFileNameWithoutExtension(fileName).Truncate(30);
        }

        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string UploadId { get; set; }
        public long FileSize { get; set; }
        public string FileExt { get; set; }
        public string Picture { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Unlisted;
        public string PrivateKey { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public void UpdateTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                this.Title = title;
            }
        }

        public void UpdateDescription(string description)
        {
            if (description is not null)
                this.Description = description;
        }
        
        public void UpdatePublicityStatus(string status)
        {
            var publicity = status.ParseToEnumOrDefault<Visibility>();
            UpdatePublicityStatus(publicity);
        }

        public void UpdatePublicityStatus(Visibility status)
        {
            this.Visibility = status;
            if (this.Visibility == Visibility.Private)
            {
                SetPrivateKey();
            }
        }

        public void SetPrivateKey()
        {
            // TODO: Use Nano-id (or shortid)
            this.PrivateKey = Guid.NewGuid().ToString("N");
        }

        public void ClearPrivateKey()
        {
            this.PrivateKey = null;
        }

        public void UpdateTags(List<Tag> tags)
        {
            if (this.Tags.Count > 0)
            {
                foreach (var audioTag in this.Tags)
                {
                    if (tags.All(t => t.Id != audioTag.Id))
                    {
                        this.Tags.Remove(audioTag);
                    }
                }

                foreach (var tag in tags)
                {
                    if (this.Tags.All(t => t.Id != tag.Id))
                        this.Tags.Add(tag);
                }
            }
            else
            {
                foreach (var tag in tags)
                {
                    this.Tags.Add(tag);
                }
            }
        }

        public void UpdatePicture(string picturePath)
        {
            if (!string.IsNullOrWhiteSpace(picturePath))
                this.Picture = picturePath;
        }

        public bool CanModify(string userId)
        {
            return this.UserId == userId;
        }
    }
}