using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBedsRooms2And3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cama",
                columns: new[] { "id_habitacion", "n_cama", "id_estado" },
                values: new object[,]
                {
                    { 2, 1, 1 },
                    { 3, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 2, 1 },
                    { 2, 3, 1 },
                    { 3, 3, 1 }
                });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$9T37o4WqxHpF1hNZkVKc.O1YW/wVcnuv2BvbQDwBoiKY1ljTksHG6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "Cama",
                keyColumns: new[] { "id_habitacion", "n_cama" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$3N/OLsw1OuMCEsB.jErDMOOhlyvYlogasyGjmjYZyOye5npd5Pthq");
        }
    }
}
