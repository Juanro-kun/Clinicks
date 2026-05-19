using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaEstadoCama : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ocupado",
                table: "Cama");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Paciente",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "id_estado",
                table: "Cama",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "EstadoCama",
                columns: table => new
                {
                    id_estado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoCama", x => x.id_estado);
                });

            migrationBuilder.InsertData(
                table: "EstadoCama",
                columns: new[] { "id_estado", "nombre" },
                values: new object[,]
                {
                    { 1, "Libre" },
                    { 2, "Ocupado" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cama_id_estado",
                table: "Cama",
                column: "id_estado");

            migrationBuilder.AddForeignKey(
                name: "FK_Cama_EstadoCama",
                table: "Cama",
                column: "id_estado",
                principalTable: "EstadoCama",
                principalColumn: "id_estado",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cama_EstadoCama",
                table: "Cama");

            migrationBuilder.DropTable(
                name: "EstadoCama");

            migrationBuilder.DropIndex(
                name: "IX_Cama_id_estado",
                table: "Cama");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "id_estado",
                table: "Cama");

            migrationBuilder.AddColumn<string>(
                name: "ocupado",
                table: "Cama",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);
        }
    }
}
