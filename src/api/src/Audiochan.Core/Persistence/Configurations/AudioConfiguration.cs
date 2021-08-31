using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Core.Persistence.Configurations
{
    public class AudioConfiguration : IEntityTypeConfiguration<Audio>
    {
        public void Configure(EntityTypeBuilder<Audio> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Slug).HasMaxLength(256);
            builder.Property(x => x.File).HasMaxLength(256);
            builder.Property(x => x.Picture).HasMaxLength(256);
            
            builder.HasIndex(x => x.Created);
            
            builder.HasMany(a => a.Favorited)
                .WithMany(u => u.FavoriteAudios)
                .UsingEntity<FavoriteAudio>(
                    j => j.HasOne(o => o.User)
                        .WithMany()
                        .HasForeignKey(o => o.UserId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne(o => o.Audio)
                        .WithMany()
                        .HasForeignKey(o => o.AudioId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey(fa => new { fa.AudioId, fa.UserId });
                    });

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