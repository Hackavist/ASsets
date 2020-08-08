using Microsoft.EntityFrameworkCore.Migrations;

namespace Assets.Migrations
{
    public partial class FormatAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetPictureFormat",
                table: "Assets",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CalibrationCertificationPictureFormat",
                table: "Assets",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetPictureFormat",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CalibrationCertificationPictureFormat",
                table: "Assets");
        }
    }
}
