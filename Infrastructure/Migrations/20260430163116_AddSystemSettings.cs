using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    SystemSettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettingKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.SystemSettingId);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "SystemSettingId", "CreatedAt", "DataType", "Description", "SettingKey", "SettingValue", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "integer", "Default VAT rate used in order total calculations.", "DefaultTaxRate", "20", null },
                    { 2, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "boolean", "Whether priority flagging is enabled in the order workflow.", "EnablePriorityOrders", "true", null },
                    { 3, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "boolean", "Whether low-value orders can bypass manual review.", "AutoApproveLowValueOrders", "false", null },
                    { 4, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "integer", "Maximum number of retry attempts for background processing jobs.", "BackgroundJobRetryLimit", "3", null },
                    { 5, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "integer", "Default date window used for the operational dashboard.", "DashboardDefaultDays", "30", null },
                    { 6, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Utc), "boolean", "Whether SDS metadata is mandatory for hazardous or restricted products.", "RequireSdsForHazardousProducts", "true", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_SettingKey",
                table: "SystemSettings",
                column: "SettingKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
