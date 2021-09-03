using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audiochan.Infrastructure.Persistence.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    picture = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    duration = table.Column<decimal>(type: "numeric", nullable: false),
                    file = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    picture = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audios", x => x.id);
                    table.ForeignKey(
                        name: "fk_audios_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "followed_users",
                columns: table => new
                {
                    observer_id = table.Column<long>(type: "bigint", nullable: false),
                    target_id = table.Column<long>(type: "bigint", nullable: false),
                    followed_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    unfollowed_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_followed_users", x => new { x.observer_id, x.target_id });
                    table.ForeignKey(
                        name: "fk_followed_users_users_observer_id",
                        column: x => x.observer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_followed_users_users_target_id",
                        column: x => x.target_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlists",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    picture = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlists", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlists_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    expiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audio_tags",
                columns: table => new
                {
                    audios_id = table.Column<long>(type: "bigint", nullable: false),
                    tags_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audio_tags", x => new { x.audios_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_audio_tags_audios_audios_id",
                        column: x => x.audios_id,
                        principalTable: "audios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_audio_tags_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorite_audios",
                columns: table => new
                {
                    audio_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    favorited = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    audio_id1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorite_audios", x => new { x.audio_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_favorite_audios_audios_audio_id",
                        column: x => x.audio_id,
                        principalTable: "audios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_favorite_audios_audios_audio_id1",
                        column: x => x.audio_id1,
                        principalTable: "audios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_favorite_audios_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorite_playlists",
                columns: table => new
                {
                    playlist_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    favorited = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    playlist_id1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorite_playlists", x => new { x.playlist_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_favorite_playlists_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_favorite_playlists_playlists_playlist_id1",
                        column: x => x.playlist_id1,
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_favorite_playlists_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlist_audios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    playlist_id = table.Column<long>(type: "bigint", nullable: false),
                    audio_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlist_audios", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlist_audios_audios_audio_id",
                        column: x => x.audio_id,
                        principalTable: "audios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_audios_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlist_tags",
                columns: table => new
                {
                    playlists_id = table.Column<long>(type: "bigint", nullable: false),
                    tags_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlist_tags", x => new { x.playlists_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_playlist_tags_playlists_playlists_id",
                        column: x => x.playlists_id,
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_tags_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audio_tags_tags_id",
                table: "audio_tags",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_audios_created",
                table: "audios",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_audios_user_id",
                table: "audios",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_audios_audio_id1",
                table: "favorite_audios",
                column: "audio_id1");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_audios_user_id",
                table: "favorite_audios",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_playlists_playlist_id1",
                table: "favorite_playlists",
                column: "playlist_id1");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_playlists_user_id",
                table: "favorite_playlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_followed_users_followed_date",
                table: "followed_users",
                column: "followed_date");

            migrationBuilder.CreateIndex(
                name: "ix_followed_users_target_id",
                table: "followed_users",
                column: "target_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_audios_audio_id",
                table: "playlist_audios",
                column: "audio_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_audios_playlist_id_audio_id",
                table: "playlist_audios",
                columns: new[] { "playlist_id", "audio_id" });

            migrationBuilder.CreateIndex(
                name: "ix_playlist_tags_tags_id",
                table: "playlist_tags",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlists_created",
                table: "playlists",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_playlists_user_id",
                table: "playlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_user_name",
                table: "users",
                column: "user_name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audio_tags");

            migrationBuilder.DropTable(
                name: "favorite_audios");

            migrationBuilder.DropTable(
                name: "favorite_playlists");

            migrationBuilder.DropTable(
                name: "followed_users");

            migrationBuilder.DropTable(
                name: "playlist_audios");

            migrationBuilder.DropTable(
                name: "playlist_tags");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "audios");

            migrationBuilder.DropTable(
                name: "playlists");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
