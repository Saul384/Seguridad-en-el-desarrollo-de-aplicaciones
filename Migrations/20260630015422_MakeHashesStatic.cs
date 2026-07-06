using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VulnerableApp.Migrations
{
    /// <inheritdoc />
    public partial class MakeHashesStatic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$wZf6qk9ODzD6jpXbs9ljhuplDCeIPd7wGxuvdBIpO7HvYccdpVUjq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$YhH73NAPzWhdxKa8gmBn4Oqybk8HpkL4OFMgR9T0.U7mIzW6BnlV.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$oV8lldHlBORG8Fyh/c8Kl.WOhHJ5ys9ipQr2NPvLScq0gdrU4gQV2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "admin");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "123456");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "password");
        }
    }
}
