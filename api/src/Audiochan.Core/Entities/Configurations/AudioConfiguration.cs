using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Core.Entities.Configurations
{
    public class AudioConfiguration : IEntityTypeConfiguration<Audio>
    {
        public void Configure(EntityTypeBuilder<Audio> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasDefaultValueSql("uuid_generate_v4()");

            builder.Property(x => x.Title)
                .HasMaxLength(100);
            builder.HasIndex(x => x.Title);

            builder.HasIndex(x => x.Created);

            builder.HasMany(a => a.Tags)
                .WithMany(t => t.Audios)
                .UsingEntity(j => j.ToTable("audio_tags"));
            
            builder.HasOne(x => x.User)
                .WithMany(x => x.Audios)
                .IsRequired()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
    public class FavoriteAudioConfiguration : IEntityTypeConfiguration<FavoriteAudio>
    {
        public void Configure(EntityTypeBuilder<FavoriteAudio> builder)
        {
            builder.HasKey(fa => new {AudioId = fa.AudioId, UserId = fa.UserId});

            builder.HasOne(o => o.User)
                .WithMany(f => f.FavoriteAudios)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Audio)
                .WithMany(f => f.Favorited)
                .HasForeignKey(o => o.AudioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(f => f.FavoriteDate);
            builder.HasQueryFilter(f => f.UnfavoriteDate == null);
        }
    }
}