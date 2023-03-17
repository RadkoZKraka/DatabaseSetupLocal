using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    /// <inheritdoc />
    public partial class ShotTableChange11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "UserModel",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "UserModel");
        }
    }
}
