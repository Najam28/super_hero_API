using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperHeroAPI.Migrations
{
    public partial class addusertables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_countries_CountryId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CountryId",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");


  

            migrationBuilder.CreateIndex(
                name: "IX_users_CountryId",
                table: "users",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_countries_CountryId",
                table: "users",
                column: "CountryId",
                principalTable: "countries",
                principalColumn: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_countries_CountryId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CountryId",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
