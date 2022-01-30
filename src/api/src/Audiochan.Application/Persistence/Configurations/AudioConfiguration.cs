using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Application.Persistence.Configurations
{
    public class AudioConfiguration : IEntityTypeConfiguration<Audio>
    {
        public void Configure(EntityTypeBuilder<Audio> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.File).HasMaxLength(256);
            builder.Property(x => x.Picture).HasMaxLength(256);

            builder.HasIndex(x => x.Title);
            builder.HasIndex(x => x.Tags).HasMethod("GIN");
            builder.HasIndex(x => x.Created);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Audios)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class FavoriteAudioConfiguration : IEntityTypeConfiguration<FavoriteAudio>
    {
        public void Configure(EntityTypeBuilder<FavoriteAudio> builder)
        {
            builder.HasKey(fa => new { fa.AudioId, fa.UserId });

            builder.HasOne(fa => fa.User)
                .WithMany(u => u.FavoriteAudios)
                .HasForeignKey(fa => fa.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fa => fa.Audio)
                .WithMany()
                .HasForeignKey(fa => fa.AudioId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}