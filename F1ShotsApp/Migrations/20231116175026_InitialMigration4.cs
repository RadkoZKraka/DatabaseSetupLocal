using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceModel_UserModel_UserShotsId",
                table: "RaceModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserModel",
                table: "UserModel");

            migrationBuilder.RenameTable(
                name: "UserModel",
                newName: "UserShotsModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserShotsModel",
                table: "UserShotsModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceModel_UserShotsModel_UserShotsId",
                table: "RaceModel",
                column: "UserShotsId",
                principalTable: "UserShotsModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceModel_UserShotsModel_UserShotsId",
                table: "RaceModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserShotsModel",
                table: "UserShotsModel");

            migrationBuilder.RenameTable(
                name: "UserShotsModel",
                newName: "UserModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserModel",
                table: "UserModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceModel_UserModel_UserShotsId",
                table: "RaceModel",
                column: "UserShotsId",
                principalTable: "UserModel",
                principalColumn: "Id");
        }
    }
}
