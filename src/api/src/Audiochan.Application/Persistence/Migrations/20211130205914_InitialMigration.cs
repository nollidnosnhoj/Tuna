using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Audiochan.Application.Persistence.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

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
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false),
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
                    followed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    unfollowed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "favorite_audios",
                columns: table => new
                {
                    audio_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    favorited = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_favorite_audios_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audios_created",
                table: "audios",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_audios_tags",
                table: "audios",
                column: "tags")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_audios_title",
                table: "audios",
                column: "title");

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
                name: "ix_followed_users_followed_date",
                table: "followed_users",
                column: "followed_date");

            migrationBuilder.CreateIndex(
                name: "ix_followed_users_target_id",
                table: "followed_users",
                column: "target_id");

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
                name: "favorite_audios");

            migrationBuilder.DropTable(
                name: "followed_users");

            migrationBuilder.DropTable(
                name: "audios");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
