using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    public partial class ShotTableChange3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PolePosition",
                table: "ShotsModel");

            migrationBuilder.DropColumn(
                name: "RaceNo",
                table: "ShotsModel");

            migrationBuilder.DropColumn(
                name: "Rand",
                table: "ShotsModel");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "ShotsModel");

            migrationBuilder.AddColumn<string>(
                name: "PolePosition",
                table: "Shots",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RaceNo",
                table: "Shots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Rand",
                table: "Shots",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Shots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PolePosition",
                table: "Shots");

            migrationBuilder.DropColumn(
                name: "RaceNo",
                table: "Shots");

            migrationBuilder.DropColumn(
                name: "Rand",
                table: "Shots");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Shots");

            migrationBuilder.AddColumn<string>(
                name: "PolePosition",
                table: "ShotsModel",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RaceNo",
                table: "ShotsModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Rand",
                table: "ShotsModel",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ShotsModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
