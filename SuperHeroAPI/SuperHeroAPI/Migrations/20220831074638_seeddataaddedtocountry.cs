using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperHeroAPI.Migrations
{
    public partial class seeddataaddedtocountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "countries",
                columns: new[] { "CountryId", "CountryName", "IOSCode" },
                values: new object[] { 1, "Pakistan", "PK" });

            migrationBuilder.InsertData(
                table: "countries",
                columns: new[] { "CountryId", "CountryName", "IOSCode" },
                values: new object[] { 2, "UAE", "UAE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "CountryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "countries",
                keyColumn: "CountryId",
                keyValue: 2);
        }
    }
}
