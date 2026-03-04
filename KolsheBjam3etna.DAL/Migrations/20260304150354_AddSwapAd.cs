using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KolsheBjam3etna.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSwapAd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SwapAds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OfferTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WantedTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwapAds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SwapAds_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SwapAds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SwapAdImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SwapAdId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwapAdImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SwapAdImages_SwapAds_SwapAdId",
                        column: x => x.SwapAdId,
                        principalTable: "SwapAds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwapAdImages_SwapAdId",
                table: "SwapAdImages",
                column: "SwapAdId");

            migrationBuilder.CreateIndex(
                name: "IX_SwapAds_CategoryId",
                table: "SwapAds",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SwapAds_UserId",
                table: "SwapAds",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwapAdImages");

            migrationBuilder.DropTable(
                name: "SwapAds");
        }
    }
}
