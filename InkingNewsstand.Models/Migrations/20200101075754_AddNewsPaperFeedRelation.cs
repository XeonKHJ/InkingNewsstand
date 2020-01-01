using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InkingNewsstand.Models.Migrations
{
    public partial class AddNewsPaperFeedRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsPapers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Icon = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPapers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsPaper_Feed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NewsPaperId = table.Column<int>(nullable: false),
                    FeedId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPaper_Feed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsPaper_Feed_Feeds_FeedId",
                        column: x => x.FeedId,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsPaper_Feed_NewsPapers_NewsPaperId",
                        column: x => x.NewsPaperId,
                        principalTable: "NewsPapers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsPaper_Feed_FeedId",
                table: "NewsPaper_Feed",
                column: "FeedId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPaper_Feed_NewsPaperId",
                table: "NewsPaper_Feed",
                column: "NewsPaperId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsPaper_Feed");

            migrationBuilder.DropTable(
                name: "NewsPapers");
        }
    }
}
