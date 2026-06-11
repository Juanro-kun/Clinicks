using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraCamas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 12345678);

            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 87654321);

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$3N/OLsw1OuMCEsB.jErDMOOhlyvYlogasyGjmjYZyOye5npd5Pthq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cama",
                columns: new[] { "id_habitacion", "n_cama", "id_estado" },
                values: new object[,]
                {
                    { 2, 1, 1 },
                    { 3, 1, 1 },
                    { 2, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Paciente",
                columns: new[] { "dni", "Activo", "apellido", "creado_por_usuario_id", "nombre", "telefono" },
                values: new object[,]
                {
                    { 12345678, true, "Pérez", 1, "Juan", "1122334455" },
                    { 87654321, true, "Gómez", 1, "María", "1155443322" }
                });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$AlS2reB/WMMI9r8NfceRCeZKd0hzv3Il.MdLFtLbZwfn00dpWE3lq");
        }
    }
}
