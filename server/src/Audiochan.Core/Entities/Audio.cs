using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class Audio : BaseEntity<string>
    {
        public Audio()
        {
            this.Tags = new HashSet<Tag>();
        }

        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string UploadId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileExt { get; set; }
        public string Picture { get; set; }
        public bool IsPublic { get; set; }
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