using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Core.Entities.Configurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            
            builder.Property(x => x.Title)
                .HasMaxLength(100);
            
            builder.HasMany(a => a.Tags)
                .WithMany(t => t.Playlists)
                .UsingEntity(j => j.ToTable("playlist_tags"));

            builder.HasOne(x => x.User)
                .WithMany(x => x.Playlists)
                .IsRequired()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(x => x.Title);
            builder.HasIndex(x => x.Created);
        }
    }
    
    public class PlaylistAudioConfiguration : IEntityTypeConfiguration<PlaylistAudio>
    {
        public void Configure(EntityTypeBuilder<PlaylistAudio> builder)
        {
            builder.HasKey(x => new {x.PlaylistId, x.AudioId});

            builder.HasOne(x => x.Playlist)
                .WithMany(x => x.Audios)
                .HasForeignKey(x => x.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Audio)
                .WithMany()
                .HasForeignKey(x => x.AudioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
    public class FavoritePlaylistConfiguration : IEntityTypeConfiguration<FavoritePlaylist>
    {
        public void Configure(EntityTypeBuilder<FavoritePlaylist> builder)
        {
            builder.HasKey(x => new {x.PlaylistId, x.UserId});

            builder.HasOne(x => x.Playlist)
                .WithMany(x => x.Favorited)
                .HasForeignKey(x => x.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(x => x.FavoritePlaylists)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}