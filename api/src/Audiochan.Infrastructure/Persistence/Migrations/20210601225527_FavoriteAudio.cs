using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audiochan.Infrastructure.Persistence.Migrations
{
    public partial class FavoriteAudio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "favorite_audios",
                columns: table => new
                {
                    audio_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    favorite_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    unfavorite_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
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
                        name: "fk_favorite_audios_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_favorite_audios_favorite_date",
                table: "favorite_audios",
                column: "favorite_date");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_audios_user_id",
                table: "favorite_audios",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_audios");
        }
    }
}
