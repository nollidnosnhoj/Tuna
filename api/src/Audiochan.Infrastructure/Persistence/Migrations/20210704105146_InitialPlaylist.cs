using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audiochan.Infrastructure.Persistence.Migrations
{
    public partial class InitialPlaylist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "playlists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    picture = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
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
                name: "favorite_playlists",
                columns: table => new
                {
                    playlist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    favorite_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    unfavorite_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
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
                    playlist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audio_id = table.Column<Guid>(type: "uuid", nullable: false),
                    added = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlist_audios", x => new { x.playlist_id, x.audio_id });
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
                    playlists_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "ix_favorite_playlists_user_id",
                table: "favorite_playlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_audios_audio_id",
                table: "playlist_audios",
                column: "audio_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_tags_tags_id",
                table: "playlist_tags",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlists_created",
                table: "playlists",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_playlists_title",
                table: "playlists",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "ix_playlists_user_id",
                table: "playlists",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_playlists");

            migrationBuilder.DropTable(
                name: "playlist_audios");

            migrationBuilder.DropTable(
                name: "playlist_tags");

            migrationBuilder.DropTable(
                name: "playlists");
        }
    }
}
