using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habitacion",
                columns: table => new
                {
                    id_habitacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitacion", x => x.id_habitacion);
                });

            migrationBuilder.CreateTable(
                name: "Paciente",
                columns: table => new
                {
                    dni = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    telefono = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paciente__D87608A6A9A48A2E", x => x.dni);
                });

            migrationBuilder.CreateTable(
                name: "Pais",
                columns: table => new
                {
                    id_pais = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pais", x => x.id_pais);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    rol = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__2ED7D2AF6D9861A0", x => x.usuario_id);
                });

            migrationBuilder.CreateTable(
                name: "Cama",
                columns: table => new
                {
                    n_cama = table.Column<int>(type: "int", nullable: false),
                    id_habitacion = table.Column<int>(type: "int", nullable: false),
                    ocupado = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cama", x => new { x.n_cama, x.id_habitacion });
                    table.ForeignKey(
                        name: "FK__Cama__id_habitac",
                        column: x => x.id_habitacion,
                        principalTable: "Habitacion",
                        principalColumn: "id_habitacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    id_provincia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    id_pais = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Provincia", x => x.id_provincia);
                    table.ForeignKey(
                        name: "FK__Provincia__id_pa",
                        column: x => x.id_pais,
                        principalTable: "Pais",
                        principalColumn: "id_pais");
                });

            migrationBuilder.CreateTable(
                name: "Internacion",
                columns: table => new
                {
                    id_internacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha_inicio = table.Column<DateTime>(type: "datetime", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "datetime", nullable: true),
                    dni = table.Column<int>(type: "int", nullable: true),
                    id_habitacion = table.Column<int>(type: "int", nullable: true),
                    n_cama = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Internacion", x => x.id_internacion);
                    table.ForeignKey(
                        name: "FK__Internacion__cama",
                        columns: x => new { x.n_cama, x.id_habitacion },
                        principalTable: "Cama",
                        principalColumns: new[] { "n_cama", "id_habitacion" });
                    table.ForeignKey(
                        name: "FK__Internacion__dni",
                        column: x => x.dni,
                        principalTable: "Paciente",
                        principalColumn: "dni");
                });

            migrationBuilder.CreateTable(
                name: "Ciudad",
                columns: table => new
                {
                    id_ciudad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    id_provincia = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ciudad", x => x.id_ciudad);
                    table.ForeignKey(
                        name: "FK__Ciudad__id_provi",
                        column: x => x.id_provincia,
                        principalTable: "Provincia",
                        principalColumn: "id_provincia");
                });

            migrationBuilder.CreateTable(
                name: "Egreso",
                columns: table => new
                {
                    id_egreso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    id_internacion = table.Column<int>(type: "int", nullable: true)
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
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    id_internacion = table.Column<int>(type: "int", nullable: true)
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
                    id_internacion_origen = table.Column<int>(type: "int", nullable: true),
                    id_internacion_destino = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "Direccion",
                columns: table => new
                {
                    id_direccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    calle = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    altura = table.Column<int>(type: "int", nullable: false),
                    id_ciudad = table.Column<int>(type: "int", nullable: true),
                    dni = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Direccion", x => x.id_direccion);
                    table.ForeignKey(
                        name: "FK_Direccion_Paciente",
                        column: x => x.dni,
                        principalTable: "Paciente",
                        principalColumn: "dni");
                    table.ForeignKey(
                        name: "FK__Direccion__id_ci",
                        column: x => x.id_ciudad,
                        principalTable: "Ciudad",
                        principalColumn: "id_ciudad");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cama_id_habitacion",
                table: "Cama",
                column: "id_habitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Ciudad_id_provincia",
                table: "Ciudad",
                column: "id_provincia");

            migrationBuilder.CreateIndex(
                name: "IX_Direccion_dni",
                table: "Direccion",
                column: "dni");

            migrationBuilder.CreateIndex(
                name: "IX_Direccion_id_ciudad",
                table: "Direccion",
                column: "id_ciudad");

            migrationBuilder.CreateIndex(
                name: "IX_Egreso_id_internacion",
                table: "Egreso",
                column: "id_internacion");

            migrationBuilder.CreateIndex(
                name: "IX_Ingreso_id_internacion",
                table: "Ingreso",
                column: "id_internacion");

            migrationBuilder.CreateIndex(
                name: "IX_Internacion_dni",
                table: "Internacion",
                column: "dni");

            migrationBuilder.CreateIndex(
                name: "IX_Internacion_n_cama_id_habitacion",
                table: "Internacion",
                columns: new[] { "n_cama", "id_habitacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Provincia_id_pais",
                table: "Provincia",
                column: "id_pais");

            migrationBuilder.CreateIndex(
                name: "IX_Traslado_id_internacion_destino",
                table: "Traslado",
                column: "id_internacion_destino");

            migrationBuilder.CreateIndex(
                name: "IX_Traslado_id_internacion_origen",
                table: "Traslado",
                column: "id_internacion_origen");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuario__AB6E6164028903F3",
                table: "Usuario",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Direccion");

            migrationBuilder.DropTable(
                name: "Egreso");

            migrationBuilder.DropTable(
                name: "Ingreso");

            migrationBuilder.DropTable(
                name: "Traslado");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Ciudad");

            migrationBuilder.DropTable(
                name: "Internacion");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Cama");

            migrationBuilder.DropTable(
                name: "Paciente");

            migrationBuilder.DropTable(
                name: "Pais");

            migrationBuilder.DropTable(
                name: "Habitacion");
        }
    }
}
