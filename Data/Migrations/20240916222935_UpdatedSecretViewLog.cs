using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomentaryMessages.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSecretViewLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialViewDate",
                table: "SecretViewLogs");

            migrationBuilder.RenameColumn(
                name: "ViewsCount",
                table: "SecretViewLogs",
                newName: "RemainingViewsCount");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "SecretViewLogs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "SecretViewLogs");

            migrationBuilder.RenameColumn(
                name: "RemainingViewsCount",
                table: "SecretViewLogs",
                newName: "ViewsCount");

            migrationBuilder.AddColumn<DateTime>(
                name: "InitialViewDate",
                table: "SecretViewLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
