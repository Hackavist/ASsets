using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Assets.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    AssetId = table.Column<string>(nullable: false),
                    AssetNumber = table.Column<string>(nullable: false),
                    AssetName = table.Column<string>(nullable: false),
                    PMVCode = table.Column<string>(nullable: true),
                    CurrentLocation = table.Column<string>(nullable: true),
                    DateOfPurchase = table.Column<DateTime>(nullable: false),
                    PoNumber = table.Column<string>(nullable: true),
                    PlateSerialNumber = table.Column<string>(nullable: true),
                    PurchaseCostOfAsset = table.Column<double>(nullable: false),
                    MonthsToDepreciation = table.Column<int>(nullable: false),
                    AssetStatus = table.Column<int>(nullable: false),
                    ToolType = table.Column<string>(nullable: false),
                    CalibrationCertificationNumber = table.Column<string>(nullable: true),
                    CalibrationCertificationDate = table.Column<DateTime>(nullable: false),
                    CalibrationCertificationPictureBase64 = table.Column<string>(nullable: true),
                    CalibrationCertificationPictureFormat = table.Column<string>(nullable: false),
                    AssetPictureBase64 = table.Column<string>(nullable: true),
                    AssetPictureFormat = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repair",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    RepairDate = table.Column<DateTime>(nullable: false),
                    LaborAmount = table.Column<double>(nullable: false),
                    SparePartsAmount = table.Column<double>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    AssetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repair_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Repositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    OldPosition = table.Column<string>(nullable: true),
                    NewPosition = table.Column<string>(nullable: true),
                    RepositionDate = table.Column<DateTime>(nullable: false),
                    AssetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repositions_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repair_AssetId",
                table: "Repair",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositions_AssetId",
                table: "Repositions",
                column: "AssetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repair");

            migrationBuilder.DropTable(
                name: "Repositions");

            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
