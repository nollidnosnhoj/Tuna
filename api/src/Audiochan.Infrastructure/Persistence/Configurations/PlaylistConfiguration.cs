using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Persistence.Configurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).HasMaxLength(100);
        }
    }

    public class PlaylistAudioConfiguration : IEntityTypeConfiguration<PlaylistAudio>
    {
        public void Configure(EntityTypeBuilder<PlaylistAudio> builder)
        {
            builder.HasKey(pa => new {AudioId = pa.AudioId, PlaylistId = pa.PlaylistId});

            builder.HasOne(o => o.Audio)
                .WithMany()
                .HasForeignKey(o => o.AudioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Playlist)
                .WithMany(f => f.Audios)
                .HasForeignKey(o => o.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Added);
        }
    }
    
    public class FavoritePlaylistConfiguration : IEntityTypeConfiguration<FavoritePlaylist>
    {
        public void Configure(EntityTypeBuilder<FavoritePlaylist> builder)
        {
            builder.HasKey(fp => new {PlaylistId = fp.PlaylistId, UserId = fp.UserId});

            builder.HasOne(o => o.User)
                .WithMany(f => f.FavoritePlaylists)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Playlist)
                .WithMany(f => f.Favoriters)
                .HasForeignKey(o => o.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(f => f.FavoriteDate);
            builder.HasQueryFilter(f => f.UnfavoriteDate == null);
        }
    }
}