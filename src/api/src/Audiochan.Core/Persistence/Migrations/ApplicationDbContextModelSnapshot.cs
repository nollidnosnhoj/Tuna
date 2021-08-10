﻿// <auto-generated />
using System;
using Audiochan.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audiochan.Core.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("AudioTag", b =>
                {
                    b.Property<long>("AudiosId")
                        .HasColumnType("bigint")
                        .HasColumnName("audios_id");

                    b.Property<long>("TagsId")
                        .HasColumnType("bigint")
                        .HasColumnName("tags_id");

                    b.HasKey("AudiosId", "TagsId")
                        .HasName("pk_audio_tags");

                    b.HasIndex("TagsId")
                        .HasDatabaseName("ix_audio_tags_tags_id");

                    b.ToTable("audio_tags");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.Audio", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<decimal>("Duration")
                        .HasColumnType("numeric")
                        .HasColumnName("duration");

                    b.Property<string>("File")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("file");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_modified");

                    b.Property<string>("Picture")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("picture");

                    b.Property<string>("Secret")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("secret");

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("size");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("slug");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("title");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.Property<int>("Visibility")
                        .HasColumnType("integer")
                        .HasColumnName("visibility");

                    b.HasKey("Id")
                        .HasName("pk_audios");

                    b.HasIndex("Created")
                        .HasDatabaseName("ix_audios_created");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_audios_user_id");

                    b.ToTable("audios");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.FavoriteAudio", b =>
                {
                    b.Property<long>("AudioId")
                        .HasColumnType("bigint")
                        .HasColumnName("audio_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("AudioId", "UserId")
                        .HasName("pk_favorite_audios");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_favorite_audios_user_id");

                    b.ToTable("favorite_audios");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.FavoritePlaylist", b =>
                {
                    b.Property<long>("PlaylistId")
                        .HasColumnType("bigint")
                        .HasColumnName("playlist_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("PlaylistId", "UserId")
                        .HasName("pk_favorite_playlists");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_favorite_playlists_user_id");

                    b.ToTable("favorite_playlists");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.FollowedUser", b =>
                {
                    b.Property<long>("ObserverId")
                        .HasColumnType("bigint")
                        .HasColumnName("observer_id");

                    b.Property<long>("TargetId")
                        .HasColumnType("bigint")
                        .HasColumnName("target_id");

                    b.Property<DateTime>("FollowedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("followed_date");

                    b.Property<DateTime?>("UnfollowedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("unfollowed_date");

                    b.HasKey("ObserverId", "TargetId")
                        .HasName("pk_followed_users");

                    b.HasIndex("FollowedDate")
                        .HasDatabaseName("ix_followed_users_followed_date");

                    b.HasIndex("TargetId")
                        .HasDatabaseName("ix_followed_users_target_id");

                    b.ToTable("followed_users");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.Playlist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_modified");

                    b.Property<string>("Picture")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("picture");

                    b.Property<string>("Secret")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("secret");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("slug");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("title");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.Property<int>("Visibility")
                        .HasColumnType("integer")
                        .HasColumnName("visibility");

                    b.HasKey("Id")
                        .HasName("pk_playlists");

                    b.HasIndex("Created")
                        .HasDatabaseName("ix_playlists_created");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_playlists_user_id");

                    b.ToTable("playlists");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.PlaylistAudio", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("AudioId")
                        .HasColumnType("bigint")
                        .HasColumnName("audio_id");

                    b.Property<long>("PlaylistId")
                        .HasColumnType("bigint")
                        .HasColumnName("playlist_id");

                    b.HasKey("Id")
                        .HasName("pk_playlist_audios");

                    b.HasIndex("AudioId")
                        .HasDatabaseName("ix_playlist_audios_audio_id");

                    b.HasIndex("PlaylistId", "AudioId")
                        .HasDatabaseName("ix_playlist_audios_playlist_id_audio_id");

                    b.ToTable("playlist_audios");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_tags");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_tags_name");

                    b.ToTable("tags");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_modified");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("Picture")
                        .HasColumnType("text")
                        .HasColumnName("picture");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("role");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasDatabaseName("ix_users_user_name");

                    b.ToTable("users");
                });

            modelBuilder.Entity("PlaylistTag", b =>
                {
                    b.Property<long>("PlaylistsId")
                        .HasColumnType("bigint")
                        .HasColumnName("playlists_id");

                    b.Property<long>("TagsId")
                        .HasColumnType("bigint")
                        .HasColumnName("tags_id");

                    b.HasKey("PlaylistsId", "TagsId")
                        .HasName("pk_playlist_tags");

                    b.HasIndex("TagsId")
                        .HasDatabaseName("ix_playlist_tags_tags_id");

                    b.ToTable("playlist_tags");
                });

            modelBuilder.Entity("AudioTag", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.Audio", null)
                        .WithMany()
                        .HasForeignKey("AudiosId")
                        .HasConstraintName("fk_audio_tags_audios_audios_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Audiochan.Core.Entities.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .HasConstraintName("fk_audio_tags_tags_tags_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Audiochan.Core.Entities.Audio", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.User", "User")
                        .WithMany("Audios")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_audios_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.FavoriteAudio", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.Audio", "Audio")
                        .WithMany("Favorited")
                        .HasForeignKey("AudioId")
                        .HasConstraintName("fk_favorite_audios_audios_audio_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Audiochan.Core.Entities.User", "User")
                        .WithMany("FavoriteAudios")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_favorite_audios_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Audio");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.FavoritePlaylist", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.Playlist", "Playlist")
                        .WithMany("Favorited")
                        .HasForeignKey("PlaylistId")
                        .HasConstraintName("fk_favorite_playlists_playlists_playlist_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Audiochan.Core.Entities.User", "User")
                        .WithMany("FavoritePlaylists")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_favorite_playlists_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Playlist");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.FollowedUser", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.User", "Observer")
                        .WithMany("Followings")
                        .HasForeignKey("ObserverId")
                        .HasConstraintName("fk_followed_users_users_observer_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Audiochan.Core.Entities.User", "Target")
                        .WithMany("Followers")
                        .HasForeignKey("TargetId")
                        .HasConstraintName("fk_followed_users_users_target_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Observer");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.Playlist", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.User", "User")
                        .WithMany("Playlists")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_playlists_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.PlaylistAudio", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.Audio", "Audio")
                        .WithMany()
                        .HasForeignKey("AudioId")
                        .HasConstraintName("fk_playlist_audios_audios_audio_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Audiochan.Core.Entities.Playlist", "Playlist")
                        .WithMany("Audios")
                        .HasForeignKey("PlaylistId")
                        .HasConstraintName("fk_playlist_audios_playlists_playlist_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Audio");

                    b.Navigation("Playlist");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.User", b =>
                {
                    b.OwnsMany("Audiochan.Core.Entities.RefreshToken", "RefreshTokens", b1 =>
                        {
                            b1.Property<long>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint")
                                .HasColumnName("id")
                                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                            b1.Property<DateTime>("Created")
                                .HasColumnType("timestamp without time zone")
                                .HasColumnName("created");

                            b1.Property<DateTime>("Expiry")
                                .HasColumnType("timestamp without time zone")
                                .HasColumnName("expiry");

                            b1.Property<string>("Token")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("token");

                            b1.Property<long>("UserId")
                                .HasColumnType("bigint")
                                .HasColumnName("user_id");

                            b1.HasKey("Id")
                                .HasName("pk_refresh_tokens");

                            b1.HasIndex("UserId")
                                .HasDatabaseName("ix_refresh_tokens_user_id");

                            b1.ToTable("refresh_tokens");

                            b1.WithOwner()
                                .HasForeignKey("UserId")
                                .HasConstraintName("fk_refresh_tokens_users_user_id");
                        });

                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("PlaylistTag", b =>
                {
                    b.HasOne("Audiochan.Core.Entities.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistsId")
                        .HasConstraintName("fk_playlist_tags_playlists_playlists_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Audiochan.Core.Entities.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .HasConstraintName("fk_playlist_tags_tags_tags_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Audiochan.Core.Entities.Audio", b =>
                {
                    b.Navigation("Favorited");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.Playlist", b =>
                {
                    b.Navigation("Audios");

                    b.Navigation("Favorited");
                });

            modelBuilder.Entity("Audiochan.Core.Entities.User", b =>
                {
                    b.Navigation("Audios");

                    b.Navigation("FavoriteAudios");

                    b.Navigation("FavoritePlaylists");

                    b.Navigation("Followers");

                    b.Navigation("Followings");

                    b.Navigation("Playlists");
                });
#pragma warning restore 612, 618
        }
    }
}
