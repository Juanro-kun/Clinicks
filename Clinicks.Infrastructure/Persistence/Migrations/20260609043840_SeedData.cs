using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Habitacion",
                columns: new[] { "id_habitacion", "nombre" },
                values: new object[,]
                {
                    { 1, "Sala General" },
                    { 2, "Sala de Emergencias" },
                    { 3, "Terapia Intensiva" }
                });

            migrationBuilder.InsertData(
                table: "Pais",
                columns: new[] { "id_pais", "nombre" },
                values: new object[] { 1, "Argentina" });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "usuario_id", "apellido", "email", "nombre", "password", "rol" },
                values: new object[] { 1, "Admin", "admin", "Admin", "1234", 1 });

            migrationBuilder.InsertData(
                table: "Cama",
                columns: new[] { "id_habitacion", "n_cama", "id_estado" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 1 },
                    { 3, 1, 1 },
                    { 1, 2, 1 },
                    { 2, 2, 1 },
                    { 1, 3, 1 }
                });

            migrationBuilder.InsertData(
                table: "Paciente",
                columns: new[] { "dni", "Activo", "apellido", "creado_por_usuario_id", "nombre", "telefono" },
                values: new object[,]
                {
                    { 45678912, true, "Lopez", 1, "Carlos", "1199887766" }
                });

            migrationBuilder.InsertData(
                table: "Provincia",
                columns: new[] { "id_provincia", "id_pais", "nombre" },
                values: new object[,]
                {
                    { 1, 1, "Buenos Aires" },
                    { 2, 1, "Córdoba" },
                    { 3, 1, "Santa Fe" }
                });

            migrationBuilder.InsertData(
                table: "Ciudad",
                columns: new[] { "id_ciudad", "id_provincia", "nombre" },
                values: new object[,]
                {
                    { 1, 1, "CABA" },
                    { 2, 1, "La Plata" },
                    { 3, 2, "Córdoba Capital" },
                    { 4, 3, "Rosario" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 1, 1 });

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
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "id_ciudad",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "id_ciudad",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "id_ciudad",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "id_ciudad",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Paciente",
                keyColumn: "dni",
                keyValue: 45678912);

            migrationBuilder.DeleteData(
                table: "Habitacion",
                keyColumn: "id_habitacion",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Habitacion",
                keyColumn: "id_habitacion",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Habitacion",
                keyColumn: "id_habitacion",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Provincia",
                keyColumn: "id_provincia",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Provincia",
                keyColumn: "id_provincia",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Provincia",
                keyColumn: "id_provincia",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pais",
                keyColumn: "id_pais",
                keyValue: 1);
        }
    }
}
