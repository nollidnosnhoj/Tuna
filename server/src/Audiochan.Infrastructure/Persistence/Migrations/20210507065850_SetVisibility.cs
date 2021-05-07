using Microsoft.EntityFrameworkCore.Migrations;

namespace Audiochan.Infrastructure.Persistence.Migrations
{
    public partial class SetVisibility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_public",
                table: "audios");

            migrationBuilder.AddColumn<int>(
                name: "visibility",
                table: "audios",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "visibility",
                table: "audios");

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                table: "audios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
