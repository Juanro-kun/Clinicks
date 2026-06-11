using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoom4AndCarlosLopez : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Habitacion",
                columns: new[] { "id_habitacion", "nombre" },
                values: new object[] { 4, "Sala de Observación" });

            migrationBuilder.UpdateData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 45678912,
                column: "apellido",
                value: "Lopez");

            migrationBuilder.InsertData(
                table: "Paciente",
                columns: new[] { "dni", "Activo", "apellido", "creado_por_usuario_id", "nombre", "telefono" },
                values: new object[,]
                {
                    { 50000001, true, "García", 1, "Ana", "1122223333" },
                    { 50000002, true, "Díaz", 1, "Bruno", "1144445555" },
                    { 50000003, true, "Torres", 1, "Cecilia", "1166667777" }
                });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$/04QedE4bTOHKu/ioEKvWeNrBtjl/Z21bysQIMhWUXjZ3E49vTFlC");

            migrationBuilder.InsertData(
                table: "Cama",
                columns: new[] { "id_habitacion", "n_cama", "id_estado" },
                values: new object[,]
                {
                    { 4, 1, 2 },
                    { 4, 2, 2 },
                    { 4, 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "Internacion",
                columns: new[] { "id_internacion", "creado_por_usuario_id", "dni", "fecha_egreso", "fecha_ingreso" },
                values: new object[,]
                {
                    { 101, 1, 50000001, null, new DateTime(2026, 6, 10, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 102, 1, 50000002, null, new DateTime(2026, 6, 10, 11, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 103, 1, 50000003, null, new DateTime(2026, 6, 10, 12, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "MovimientoCama",
                columns: new[] { "id_movimiento", "fecha_fin", "fecha_inicio", "id_habitacion", "id_internacion", "n_cama" },
                values: new object[,]
                {
                    { 101, null, new DateTime(2026, 6, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), 4, 101, 1 },
                    { 102, null, new DateTime(2026, 6, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), 4, 102, 2 },
                    { 103, null, new DateTime(2026, 6, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), 4, 103, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MovimientoCama",
                keyColumn: "id_movimiento",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "MovimientoCama",
                keyColumn: "id_movimiento",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "MovimientoCama",
                keyColumn: "id_movimiento",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "Internacion",
                keyColumn: "id_internacion",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Internacion",
                keyColumn: "id_internacion",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Internacion",
                keyColumn: "id_internacion",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Habitacion",
                keyColumn: "id_habitacion",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 50000001);

            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 50000002);

            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 50000003);

            migrationBuilder.UpdateData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 45678912,
                column: "apellido",
                value: "Rodríguez");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$9T37o4WqxHpF1hNZkVKc.O1YW/wVcnuv2BvbQDwBoiKY1ljTksHG6");
        }
    }
}
