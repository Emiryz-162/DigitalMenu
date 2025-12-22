using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMenu.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBackgroundImageFromSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImagePath",
                table: "Settings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundImagePath",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Settings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BackgroundImagePath",
                value: "/images/settings/default-bg.jpg");
        }
    }
}
