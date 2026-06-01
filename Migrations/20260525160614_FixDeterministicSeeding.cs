using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotnetCRUD.Migrations
{
    /// <inheritdoc />
    public partial class FixDeterministicSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "admin@autocare.local", "$2a$11$3bGgCW.rYl4QNY8HMtnbRuDgSHXDIxwxt867EsuninAJUlw4x754y", "ADMIN", "admin" },
                    { 2, "mechanic@autocare.local", "$2a$11$rTSXCDetpkIJnbmw8Euktuo0pQB469kyn0AnuNO/TmN9BZpiLFCN.", "MECHANIC", "mechanic01" },
                    { 3, "customer@autocare.local", "$2a$11$dN.qAjMwj.2UILp.h0zw0u9xyjzPvZYXlCF7zhdbtTDLtF6ajKxQC", "CUSTOMER", "customer01" }
                });
        }
    }
}
