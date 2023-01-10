using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    public partial class ShotTableChange5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "No",
                table: "ShotsModel");

            migrationBuilder.AddColumn<int>(
                name: "No",
                table: "Shots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "No",
                table: "Shots");

            migrationBuilder.AddColumn<int>(
                name: "No",
                table: "ShotsModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
