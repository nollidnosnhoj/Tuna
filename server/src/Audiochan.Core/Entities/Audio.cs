using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class Audio : BaseEntity<string>
    {
        public Audio()
        {
            this.IsPublic = false;
            this.IsPublish = false;
            this.Tags = new HashSet<Tag>();
        }

        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Duration { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public long FileSize { get; set; }
        public string FileExt { get; set; }
        public string Picture { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPublish { get; set; }
        public DateTime? PublishDate { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public void UpdateTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                this.Title = title;
        }

        public void UpdateDescription(string description)
        {
            if (description is not null)
                this.Description = description;
        }

        public void UpdatePublicity(bool isPublic)
        {
            this.IsPublic = isPublic;
        }

        public void PublishAudio(DateTime publishTime)
        {
            if (!this.IsPublish)
            {
                this.IsPublish = true;
                this.PublishDate = publishTime;
            }
        }

        public void UnPublishAudio()
        {
            if (this.IsPublish)
            {
                this.IsPublish = false;
                this.PublishDate = null;
            }
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