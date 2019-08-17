using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InkingNewsstand.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feeds",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsPapers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaperTitle = table.Column<string>(nullable: true),
                    ExtendMode = table.Column<bool>(nullable: false),
                    IconType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPapers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FeedId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Authors = table.Column<string>(nullable: true),
                    PublishedDate = table.Column<DateTimeOffset>(nullable: false),
                    Summary = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    NewsLink = table.Column<string>(nullable: true),
                    CoverUrl = table.Column<string>(nullable: true),
                    InnerHTML = table.Column<string>(nullable: true),
                    ExtendedHtml = table.Column<string>(nullable: true),
                    InkStrokes = table.Column<byte[]>(nullable: true),
                    IsFavorite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_Feeds_FeedId",
                        column: x => x.FeedId,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsPaper_Feed",
                columns: table => new
                {
                    NewsPaperId = table.Column<int>(nullable: false),
                    FeedId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPaper_Feed", x => new { x.FeedId, x.NewsPaperId });
                    table.ForeignKey(
                        name: "FK_NewsPaper_Feed_Feeds_FeedId",
                        column: x => x.FeedId,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsPaper_Feed_NewsPapers_NewsPaperId",
                        column: x => x.NewsPaperId,
                        principalTable: "NewsPapers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_FeedId",
                table: "News",
                column: "FeedId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPaper_Feed_NewsPaperId",
                table: "NewsPaper_Feed",
                column: "NewsPaperId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "NewsPaper_Feed");

            migrationBuilder.DropTable(
                name: "Feeds");

            migrationBuilder.DropTable(
                name: "NewsPapers");
        }
    }
}
