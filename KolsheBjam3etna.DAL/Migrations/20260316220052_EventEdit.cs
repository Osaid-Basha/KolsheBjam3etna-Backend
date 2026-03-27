using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KolsheBjam3etna.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EventEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClubName",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClubName",
                table: "Events");
        }
    }
}
