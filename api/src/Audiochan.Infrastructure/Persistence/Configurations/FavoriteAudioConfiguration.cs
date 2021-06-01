using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Persistence.Configurations
{
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