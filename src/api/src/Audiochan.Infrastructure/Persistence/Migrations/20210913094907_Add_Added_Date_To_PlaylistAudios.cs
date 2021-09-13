using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audiochan.Infrastructure.Persistence.Migrations
{
    public partial class Add_Added_Date_To_PlaylistAudios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "added_by",
                table: "playlist_audios",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "added_by",
                table: "playlist_audios");
        }
    }
}
