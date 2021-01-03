using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Data.Configurations
{
    public class FavoriteAudioConfiguration : IEntityTypeConfiguration<FavoriteAudio>
    {
        public void Configure(EntityTypeBuilder<FavoriteAudio> builder)
        {
            builder
                .HasKey(ft => new { ft.UserId, ft.AudioId });

            builder
                .HasOne(ft => ft.Audio)
                .WithMany(t => t.Favorited)
                .HasForeignKey(ft => ft.AudioId);

            builder
                .HasOne(ft => ft.User)
                .WithMany(u => u.FavoriteAudios)
                .HasForeignKey(ft => ft.UserId);
        }
    }
}
