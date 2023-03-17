using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    /// <inheritdoc />
    public partial class ShotTableChange10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "RaceModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locked",
                table: "RaceModel");
        }
    }
}
