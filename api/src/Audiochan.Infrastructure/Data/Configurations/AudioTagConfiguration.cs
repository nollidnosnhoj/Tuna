using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Data.Configurations
{
    public class AudioTagConfiguration : IEntityTypeConfiguration<AudioTag>
    {
        public void Configure(EntityTypeBuilder<AudioTag> builder)
        {
            builder
                .HasKey(tt => new { tt.AudioId, tt.TagId });

            builder
                .HasOne(tt => tt.Audio)
                .WithMany(audio => audio.Tags)
                .HasForeignKey(tt => tt.AudioId);

            builder
                .HasOne(tt => tt.Tag)
                .WithMany(tag => tag.Audios)
                .HasForeignKey(tt => tt.TagId);

        }
    }
}
