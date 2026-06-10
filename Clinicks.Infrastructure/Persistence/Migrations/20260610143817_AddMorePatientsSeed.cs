using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMorePatientsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Paciente",
                columns: new[] { "dni", "Activo", "apellido", "creado_por_usuario_id", "nombre", "telefono" },
                values: new object[,]
                {
                    { 22334455, true, "Martínez", 1, "Diego", "1166778899" },
                    { 33445566, true, "Fernández", 1, "Laura", "1144556677" }
                });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$AlS2reB/WMMI9r8NfceRCeZKd0hzv3Il.MdLFtLbZwfn00dpWE3lq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 22334455);

            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 33445566);

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$s6C3b/e0jIB988uFWKwJruxrPvHViFqdJJdvPjLF2gy0LmphVAr6u");
        }
    }
}
