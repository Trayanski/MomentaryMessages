using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomentaryMessages.Data.Migrations
{
  /// <inheritdoc />
  public partial class AddingSecretTable : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "SecretViewLogs",
          columns: table => new
          {
            ViewerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
            InitialViewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            ViewsCount = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_SecretViewLogs", x => x.ViewerName);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "SecretViewLogs");
    }
  }
}
