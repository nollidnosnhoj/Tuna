using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Data.Configurations
{
    public class AudioConfiguration : IEntityTypeConfiguration<Audio>
    {
        public void Configure(EntityTypeBuilder<Audio> builder)
        {
            builder.Property(x => x.Title)
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.FileExt)
                .HasMaxLength(10);

            builder.HasMany(a => a.Tags)
                .WithMany(t => t.Audios)
                .UsingEntity(j => j.ToTable("audio_tags"));

            builder.HasOne(x => x.User)
                .WithMany(x => x.Audios)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
