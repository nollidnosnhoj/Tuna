using Microsoft.EntityFrameworkCore.Migrations;

namespace Audiochan.Infrastructure.Persistence.Migrations
{
    public partial class Followers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_followed_users",
                table: "followed_users");

            migrationBuilder.DropIndex(
                name: "ix_followed_users_target_id",
                table: "followed_users");

            migrationBuilder.AddPrimaryKey(
                name: "pk_followed_users",
                table: "followed_users",
                columns: new[] { "target_id", "observer_id" });

            migrationBuilder.CreateIndex(
                name: "ix_followed_users_followed_date",
                table: "followed_users",
                column: "followed_date");

            migrationBuilder.CreateIndex(
                name: "ix_followed_users_observer_id",
                table: "followed_users",
                column: "observer_id");

            migrationBuilder.CreateIndex(
                name: "ix_followed_users_unfollowed_date",
                table: "followed_users",
                column: "unfollowed_date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_followed_users",
                table: "followed_users");

            migrationBuilder.DropIndex(
                name: "ix_followed_users_followed_date",
                table: "followed_users");

            migrationBuilder.DropIndex(
                name: "ix_followed_users_observer_id",
                table: "followed_users");

            migrationBuilder.DropIndex(
                name: "ix_followed_users_unfollowed_date",
                table: "followed_users");

            migrationBuilder.AddPrimaryKey(
                name: "pk_followed_users",
                table: "followed_users",
                columns: new[] { "observer_id", "target_id" });

            migrationBuilder.CreateIndex(
                name: "ix_followed_users_target_id",
                table: "followed_users",
                column: "target_id");
        }
    }
}
