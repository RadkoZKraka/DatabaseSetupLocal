using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    public partial class ShotTableChange7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceModel_ShotsModel_UserId",
                table: "RaceModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShotsModel",
                table: "ShotsModel");

            migrationBuilder.DropColumn(
                name: "PolePosition",
                table: "ShotModel");

            migrationBuilder.DropColumn(
                name: "Rand",
                table: "ShotModel");

            migrationBuilder.RenameTable(
                name: "ShotsModel",
                newName: "UserModel");

            migrationBuilder.AddColumn<string>(
                name: "PolePosition",
                table: "RaceModel",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RaceNo",
                table: "RaceModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RaceYear",
                table: "RaceModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Rand",
                table: "RaceModel",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserModel",
                table: "UserModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceModel_UserModel_UserId",
                table: "RaceModel",
                column: "UserId",
                principalTable: "UserModel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceModel_UserModel_UserId",
                table: "RaceModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserModel",
                table: "UserModel");

            migrationBuilder.DropColumn(
                name: "PolePosition",
                table: "RaceModel");

            migrationBuilder.DropColumn(
                name: "RaceNo",
                table: "RaceModel");

            migrationBuilder.DropColumn(
                name: "RaceYear",
                table: "RaceModel");

            migrationBuilder.DropColumn(
                name: "Rand",
                table: "RaceModel");

            migrationBuilder.RenameTable(
                name: "UserModel",
                newName: "ShotsModel");

            migrationBuilder.AddColumn<string>(
                name: "PolePosition",
                table: "ShotModel",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Rand",
                table: "ShotModel",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShotsModel",
                table: "ShotsModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceModel_ShotsModel_UserId",
                table: "RaceModel",
                column: "UserId",
                principalTable: "ShotsModel",
                principalColumn: "Id");
        }
    }
}
