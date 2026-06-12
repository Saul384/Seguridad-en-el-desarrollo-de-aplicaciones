using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VulnerableApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        // 1. Declaramos el arreglo como static readonly al nivel de la clase
        private static readonly string[] ColumnasUsuarios = new[] { "Id", "Balance", "CreatedAt", "Email", "Password", "Username" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: ColumnasUsuarios, // 2. Usamos la variable aquí
                values: new object[,]
                {
                    { 1, 1000m, new DateTime(2026, 6, 1, 19, 54, 10, 640, DateTimeKind.Local).AddTicks(4037), "admin@test.com", "admin", "admin" },
                    { 2, 500m, new DateTime(2026, 6, 1, 19, 54, 10, 643, DateTimeKind.Local).AddTicks(2461), "user@test.com", "123456", "user1" },
                    { 3, 750m, new DateTime(2026, 6, 1, 19, 54, 10, 643, DateTimeKind.Local).AddTicks(2485), "user2@test.com", "password", "user2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}