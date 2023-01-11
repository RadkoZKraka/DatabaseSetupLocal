using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    public partial class ShotTableChange9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Driver",
                table: "ShotModel",
                newName: "UserDriver");

            migrationBuilder.AddColumn<bool>(
                name: "Result",
                table: "ShotModel",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultDriver",
                table: "ShotModel",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "RaceModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "ShotModel");

            migrationBuilder.DropColumn(
                name: "ResultDriver",
                table: "ShotModel");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "RaceModel");

            migrationBuilder.RenameColumn(
                name: "UserDriver",
                table: "ShotModel",
                newName: "Driver");
        }
    }
}
