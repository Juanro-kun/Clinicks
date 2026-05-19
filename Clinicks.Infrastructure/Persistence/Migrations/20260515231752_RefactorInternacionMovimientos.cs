using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorInternacionMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Internacion__cama",
                table: "Internacion");

            migrationBuilder.DropTable(
                name: "Egreso");

            migrationBuilder.DropTable(
                name: "Ingreso");

            migrationBuilder.DropTable(
                name: "Traslado");

            migrationBuilder.DropIndex(
                name: "IX_Internacion_n_cama_id_habitacion",
                table: "Internacion");

            migrationBuilder.DropColumn(
                name: "id_habitacion",
                table: "Internacion");

            migrationBuilder.DropColumn(
                name: "n_cama",
                table: "Internacion");

            migrationBuilder.RenameColumn(
                name: "fecha_inicio",
                table: "Internacion",
                newName: "fecha_ingreso");

            migrationBuilder.RenameColumn(
                name: "fecha_fin",
                table: "Internacion",
                newName: "fecha_egreso");

            migrationBuilder.CreateTable(
                name: "MovimientoCama",
                columns: table => new
                {
                    id_movimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_internacion = table.Column<int>(type: "int", nullable: false),
                    id_habitacion = table.Column<int>(type: "int", nullable: false),
                    n_cama = table.Column<int>(type: "int", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MovimientoCama", x => x.id_movimiento);
                    table.ForeignKey(
                        name: "FK__MovimientoCama__cama",
                        columns: x => new { x.n_cama, x.id_habitacion },
                        principalTable: "Cama",
                        principalColumns: new[] { "n_cama", "id_habitacion" });
                    table.ForeignKey(
                        name: "FK__MovimientoCama__id_habitacion",
                        column: x => x.id_habitacion,
                        principalTable: "Habitacion",
                        principalColumn: "id_habitacion");
                    table.ForeignKey(
                        name: "FK__MovimientoCama__id_internacion",
                        column: x => x.id_internacion,
                        principalTable: "Internacion",
                        principalColumn: "id_internacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCama_id_habitacion",
                table: "MovimientoCama",
                column: "id_habitacion");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCama_id_internacion",
                table: "MovimientoCama",
                column: "id_internacion");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCama_n_cama_id_habitacion",
                table: "MovimientoCama",
                columns: new[] { "n_cama", "id_habitacion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientoCama");

            migrationBuilder.RenameColumn(
                name: "fecha_ingreso",
                table: "Internacion",
                newName: "fecha_inicio");

            migrationBuilder.RenameColumn(
                name: "fecha_egreso",
                table: "Internacion",
                newName: "fecha_fin");

            migrationBuilder.AddColumn<int>(
                name: "id_habitacion",
                table: "Internacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "n_cama",
                table: "Internacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Egreso",
                columns: table => new
                {
                    id_egreso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_internacion = table.Column<int>(type: "int", nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Egreso", x => x.id_egreso);
                    table.ForeignKey(
                        name: "FK__Egreso__id_inter",
                        column: x => x.id_internacion,
                        principalTable: "Internacion",
                        principalColumn: "id_internacion");
                });

            migrationBuilder.CreateTable(
                name: "Ingreso",
                columns: table => new
                {
                    id_ingreso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_internacion = table.Column<int>(type: "int", nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ingreso", x => x.id_ingreso);
                    table.ForeignKey(
                        name: "FK__Ingreso__id_inte",
                        column: x => x.id_internacion,
                        principalTable: "Internacion",
                        principalColumn: "id_internacion");
                });

            migrationBuilder.CreateTable(
                name: "Traslado",
                columns: table => new
                {
                    id_traslado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_internacion_destino = table.Column<int>(type: "int", nullable: true),
                    id_internacion_origen = table.Column<int>(type: "int", nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Traslado", x => x.id_traslado);
                    table.ForeignKey(
                        name: "FK__Traslado__destin",
                        column: x => x.id_internacion_destino,
                        principalTable: "Internacion",
                        principalColumn: "id_internacion");
                    table.ForeignKey(
                        name: "FK__Traslado__origen",
                        column: x => x.id_internacion_origen,
                        principalTable: "Internacion",
                        principalColumn: "id_internacion");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Internacion_n_cama_id_habitacion",
                table: "Internacion",
                columns: new[] { "n_cama", "id_habitacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Egreso_id_internacion",
                table: "Egreso",
                column: "id_internacion");

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_id_internacion",
                table: "Ingreso",
                column: "id_internacion");

            migrationBuilder.CreateIndex(
                name: "IX_Traslado_id_internacion_destino",
                table: "Traslado",
                column: "id_internacion_destino");

            migrationBuilder.CreateIndex(
                name: "IX_Traslado_id_internacion_origen",
                table: "Traslado",
                column: "id_internacion_origen");

            migrationBuilder.AddForeignKey(
                name: "FK__Internacion__cama",
                table: "Internacion",
                columns: new[] { "n_cama", "id_habitacion" },
                principalTable: "Cama",
                principalColumns: new[] { "n_cama", "id_habitacion" });
        }
    }
}
