using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KolsheBjam3etna.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAtUtc",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileContentType",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EditedAtUtc",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FileContentType",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Messages");
        }
    }
}
