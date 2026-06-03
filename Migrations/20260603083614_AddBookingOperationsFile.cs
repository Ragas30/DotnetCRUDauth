using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotnetCRUD.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingOperationsFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceCatalogs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceCatalogs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceCatalogs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecommendedNextServiceDate",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedNextServiceMileage",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceNotes",
                table: "Bookings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RecommendedNextServiceDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RecommendedNextServiceMileage",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ServiceNotes",
                table: "Bookings");

            migrationBuilder.InsertData(
                table: "ServiceCatalogs",
                columns: new[] { "Id", "BasePrice", "CreatedAt", "CreatedBy", "DurationMinutes", "IsActive", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 450000m, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "system", 60, true, "Ganti Oli + Filter", null, null },
                    { 2, 850000m, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "system", 120, true, "Tune Up", null, null },
                    { 3, 250000m, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "system", 45, true, "General Checkup", null, null }
                });
        }
    }
}
