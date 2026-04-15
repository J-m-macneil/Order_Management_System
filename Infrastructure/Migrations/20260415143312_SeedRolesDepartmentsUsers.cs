using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesDepartmentsUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Roles",
                newName: "RoleId");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Users",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "Name" },
                values: new object[,]
                {
                    { 1, "IT" },
                    { 2, "Sales" },
                    { 3, "Operations" },
                    { 4, "Customer Service" },
                    { 5, "Finance" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Sales" },
                    { 3, "Operations" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "DepartmentId", "Email", "FirstName", "FullName", "IsActive", "JobTitle", "LastLoginAt", "LastName", "PasswordHash", "RoleId", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 17, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, "sarah.bennett@chemflow.local", "Sarah", "Sarah Bennett", true, "Head of Business Systems", new DateTime(2026, 3, 21, 8, 15, 0, 0, DateTimeKind.Unspecified), "Bennett", "Password123!", 1, "sbennett" },
                    { 2, new DateTime(2024, 1, 29, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, "james.carter@chemflow.local", "James", "James Carter", true, "IT Systems Administrator", new DateTime(2026, 4, 10, 3, 15, 0, 0, DateTimeKind.Unspecified), "Carter", "Password123!", 1, "jcarter" },
                    { 3, new DateTime(2024, 2, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, "olivia.hughes@chemflow.local", "Olivia", "Olivia Hughes", true, "Account Manager", new DateTime(2026, 4, 2, 7, 15, 0, 0, DateTimeKind.Unspecified), "Hughes", "Password123!", 2, "ohughes" },
                    { 4, new DateTime(2024, 2, 22, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, "daniel.foster@chemflow.local", "Daniel", "Daniel Foster", true, "Internal Sales Executive", new DateTime(2026, 4, 3, 7, 15, 0, 0, DateTimeKind.Unspecified), "Foster", "Password123!", 2, "dfoster" },
                    { 5, new DateTime(2024, 3, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, "megan.patel@chemflow.local", "Megan", "Megan Patel", true, "Sales Coordinator", new DateTime(2026, 3, 18, 8, 15, 0, 0, DateTimeKind.Unspecified), "Patel", "Password123!", 2, "mpatel" },
                    { 6, new DateTime(2024, 3, 17, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, "thomas.green@chemflow.local", "Thomas", "Thomas Green", true, "Regional Sales Representative", new DateTime(2026, 3, 20, 3, 15, 0, 0, DateTimeKind.Unspecified), "Green", "Password123!", 2, "tgreen" },
                    { 7, new DateTime(2024, 3, 29, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "rachel.morgan@chemflow.local", "Rachel", "Rachel Morgan", true, "Operations Planner", new DateTime(2026, 3, 24, 8, 15, 0, 0, DateTimeKind.Unspecified), "Morgan", "Password123!", 3, "rmorgan" },
                    { 8, new DateTime(2024, 4, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "ben.turner@chemflow.local", "Ben", "Ben Turner", true, "Logistics Coordinator", new DateTime(2026, 3, 23, 5, 15, 0, 0, DateTimeKind.Unspecified), "Turner", "Password123!", 3, "bturner" },
                    { 9, new DateTime(2024, 4, 22, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "emily.scott@chemflow.local", "Emily", "Emily Scott", true, "Order Processing Specialist", new DateTime(2026, 4, 9, 8, 15, 0, 0, DateTimeKind.Unspecified), "Scott", "Password123!", 3, "escott" },
                    { 10, new DateTime(2024, 5, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "nathan.price@chemflow.local", "Nathan", "Nathan Price", true, "Warehouse & Dispatch Coordinator", new DateTime(2026, 4, 8, 7, 15, 0, 0, DateTimeKind.Unspecified), "Price", "Password123!", 3, "nprice" },
                    { 11, new DateTime(2024, 5, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "chloe.evans@chemflow.local", "Chloe", "Chloe Evans", true, "Customer Fulfilment Analyst", new DateTime(2026, 4, 3, 4, 15, 0, 0, DateTimeKind.Unspecified), "Evans", "Password123!", 3, "cevans" },
                    { 12, new DateTime(2024, 5, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, "laura.jenkins@chemflow.local", "Laura", "Laura Jenkins", false, "Former Account Manager", null, "Jenkins", "Password123!", 2, "ljenkins" },
                    { 13, new DateTime(2024, 6, 9, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "matthew.collins@chemflow.local", "Matthew", "Matthew Collins", false, "Former Logistics Coordinator", null, "Collins", "Password123!", 3, "mcollins" },
                    { 14, new DateTime(2024, 6, 21, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, "sophie.ward@chemflow.local", "Sophie", "Sophie Ward", false, "Former Systems Analyst", null, "Ward", "Password123!", 1, "sward" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Users_DepartmentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Roles",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
