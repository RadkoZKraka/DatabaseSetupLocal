using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations.F1ShotsUser
{
    /// <inheritdoc />
    public partial class CreateCustomUserData6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "F1ShotsUserId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "F1ShotsUserId",
                table: "AspNetUsers");
        }
    }
}
