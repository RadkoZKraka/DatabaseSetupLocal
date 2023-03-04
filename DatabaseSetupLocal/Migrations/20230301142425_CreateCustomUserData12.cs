using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    /// <inheritdoc />
    public partial class CreateCustomUserData12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceModel_UserModel_UserId",
                table: "RaceModel");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RaceModel",
                newName: "UserShotsId");

            migrationBuilder.RenameIndex(
                name: "IX_RaceModel_UserId",
                table: "RaceModel",
                newName: "IX_RaceModel_UserShotsId");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceModel_UserModel_UserShotsId",
                table: "RaceModel",
                column: "UserShotsId",
                principalTable: "UserModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceModel_UserModel_UserShotsId",
                table: "RaceModel");

            migrationBuilder.RenameColumn(
                name: "UserShotsId",
                table: "RaceModel",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RaceModel_UserShotsId",
                table: "RaceModel",
                newName: "IX_RaceModel_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceModel_UserModel_UserId",
                table: "RaceModel",
                column: "UserId",
                principalTable: "UserModel",
                principalColumn: "Id");
        }
    }
}
