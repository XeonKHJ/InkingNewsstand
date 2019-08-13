using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InkingNewsstand.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsPapers",
                columns: table => new
                {
                    PaperTitle = table.Column<string>(nullable: false),
                    ExtendMode = table.Column<bool>(nullable: false),
                    IconType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPapers", x => x.PaperTitle);
                });

            migrationBuilder.CreateTable(
                name: "Feeds",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    NewsPaperPaperTitle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds", x => x.Id);
                    table.ForeignKey(
                        name: "ManyToOne_OneFeedBelongsToManyNewsPapers",
                        column: x => x.NewsPaperPaperTitle,
                        principalTable: "NewsPapers",
                        principalColumn: "PaperTitle",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Title = table.Column<string>(nullable: false),
                    PublishedDate = table.Column<DateTimeOffset>(nullable: false),
                    Authors = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    FeedId = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    NewsLink = table.Column<string>(nullable: true),
                    NewsPaperPaperTitle = table.Column<string>(nullable: true),
                    CoverUrl = table.Column<string>(nullable: true),
                    InnerHTML = table.Column<string>(nullable: true),
                    ExtendedHtml = table.Column<string>(nullable: true),
                    InkStrokes = table.Column<byte[]>(nullable: true),
                    IsFavorite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompositePrimaryKey_News", x => new { x.Title, x.Authors, x.PublishedDate });
                    table.ForeignKey(
                        name: "OneToMany_OneFeedContainsManyNews",
                        column: x => x.FeedId,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_News_NewsPapers_NewsPaperPaperTitle",
                        column: x => x.NewsPaperPaperTitle,
                        principalTable: "NewsPapers",
                        principalColumn: "PaperTitle",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_NewsPaperPaperTitle",
                table: "Feeds",
                column: "NewsPaperPaperTitle");

            migrationBuilder.CreateIndex(
                name: "IX_News_FeedId",
                table: "News",
                column: "FeedId");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsPaperPaperTitle",
                table: "News",
                column: "NewsPaperPaperTitle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Feeds");

            migrationBuilder.DropTable(
                name: "NewsPapers");
        }
    }
}
