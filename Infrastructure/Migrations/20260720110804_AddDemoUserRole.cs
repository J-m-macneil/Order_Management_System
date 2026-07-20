using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Name" },
                values: new object[] { 4, "Demo" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "DepartmentId", "Email", "FirstName", "FullName", "IsActive", "JobTitle", "LastLoginAt", "LastName", "PasswordHash", "RoleId", "Username" },
                values: new object[] { 15, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "demo@back.software", "Demo", "Demo User", true, "Read-only Demonstration Account", null, "User", "AQAAAAIAAYagAAAAEAxPHh/D5dKq1ysW0WfcNd1UoSgMITFPlUGStQOPuEraeGeQXO+sxp+PNvm2QILaWQ==", 4, "demo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4);
        }
    }
}
