using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    /// <inheritdoc />
    public partial class CreateCustomUserData5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RaceLocation = table.Column<string>(type: "TEXT", nullable: true),
                    RaceYear = table.Column<int>(type: "INTEGER", nullable: false),
                    RaceNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Rand = table.Column<string>(type: "TEXT", nullable: true),
                    PolePosition = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceModel_UserModel_UserId",
                        column: x => x.UserId,
                        principalTable: "UserModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShotModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserDriver = table.Column<string>(type: "TEXT", nullable: true),
                    ResultDriver = table.Column<string>(type: "TEXT", nullable: true),
                    Result = table.Column<bool>(type: "INTEGER", nullable: true),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShotModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShotModel_RaceModel_RaceId",
                        column: x => x.RaceId,
                        principalTable: "RaceModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaceModel_UserId",
                table: "RaceModel",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShotModel_RaceId",
                table: "ShotModel",
                column: "RaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShotModel");

            migrationBuilder.DropTable(
                name: "RaceModel");

            migrationBuilder.DropTable(
                name: "UserModel");
        }
    }
}
