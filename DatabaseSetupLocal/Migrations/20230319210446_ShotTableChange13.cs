using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    /// <inheritdoc />
    public partial class ShotTableChange13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "ShotModel");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "ShotModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "ShotModel");

            migrationBuilder.AddColumn<bool>(
                name: "Result",
                table: "ShotModel",
                type: "INTEGER",
                nullable: true);
        }
    }
}
