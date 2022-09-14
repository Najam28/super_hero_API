using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperHeroAPI.Migrations
{
    public partial class addforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "countries",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "IOSCode",
                value: "ARE");

            migrationBuilder.CreateIndex(
                name: "IX_users_CountryId",
                table: "users",
                column: "CountryId",
                unique: false);

            migrationBuilder.AddForeignKey(
                name: "FK_users_countries_CountryId",
                table: "users",
                column: "CountryId",
                principalTable: "countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_countries_CountryId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CountryId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "countries",
                keyColumn: "CountryId",
                keyValue: 2,
                column: "IOSCode",
                value: "UAE");


        }
    }
}
