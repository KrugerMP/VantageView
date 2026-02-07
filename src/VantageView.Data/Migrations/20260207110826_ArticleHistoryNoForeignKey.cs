using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VantageView.Data.Migrations
{
    /// <inheritdoc />
    public partial class ArticleHistoryNoForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleHistory_ArticleId",
                table: "ArticleHistory",
                column: "ArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleHistory");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
