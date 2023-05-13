using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseSetupLocal.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "RaceModel");

            migrationBuilder.AddColumn<bool>(
                name: "Banned",
                table: "UserModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "UserModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Banned",
                table: "UserModel");

            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "UserModel");

            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "RaceModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
