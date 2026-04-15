using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HashSeededUserPasswordsRunTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 10,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 11,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 12,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 13,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 14,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 10,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 11,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 12,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 13,
                column: "PasswordHash",
                value: "Password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 14,
                column: "PasswordHash",
                value: "Password123!");
        }
    }
}
