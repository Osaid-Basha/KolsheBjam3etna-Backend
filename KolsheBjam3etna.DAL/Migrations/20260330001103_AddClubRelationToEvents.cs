using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KolsheBjam3etna.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClubRelationToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_CoordinatorId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CoordinatorId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ClubName",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CoordinatorId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "ManagedEventId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UniversityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ManagerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SubscriptionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubscriptionPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubscriptionStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriptionEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clubs_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ManagedEventId",
                table: "Users",
                column: "ManagedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ClubId",
                table: "Events",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_OwnerId",
                table: "Clubs",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Clubs_ClubId",
                table: "Events",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Events_ManagedEventId",
                table: "Users",
                column: "ManagedEventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Clubs_ClubId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Events_ManagedEventId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropIndex(
                name: "IX_Users_ManagedEventId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Events_ClubId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ManagedEventId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "ClubName",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CoordinatorId",
                table: "Events",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CoordinatorId",
                table: "Events",
                column: "CoordinatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_CoordinatorId",
                table: "Events",
                column: "CoordinatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
