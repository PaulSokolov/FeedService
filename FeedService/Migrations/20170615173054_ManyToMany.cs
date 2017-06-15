using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FeedService.Migrations
{
    public partial class ManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feeds_Collections_CollectionId",
                table: "Feeds");

            migrationBuilder.DropIndex(
                name: "IX_Feeds_CollectionId",
                table: "Feeds");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Feeds");

            migrationBuilder.CreateTable(
                name: "CollectionFeed",
                columns: table => new
                {
                    CollectionId = table.Column<int>(nullable: false),
                    FeedId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionFeed", x => new { x.CollectionId, x.FeedId });
                    table.ForeignKey(
                        name: "FK_CollectionFeed_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionFeed_Feeds_FeedId",
                        column: x => x.FeedId,
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionFeed_FeedId",
                table: "CollectionFeed",
                column: "FeedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionFeed");

            migrationBuilder.AddColumn<int>(
                name: "CollectionId",
                table: "Feeds",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_CollectionId",
                table: "Feeds",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeds_Collections_CollectionId",
                table: "Feeds",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
