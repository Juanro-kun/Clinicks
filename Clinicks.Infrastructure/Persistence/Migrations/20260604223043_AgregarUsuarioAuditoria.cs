using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUsuarioAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "creado_por_usuario_id",
                table: "Paciente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "creado_por_usuario_id",
                table: "Internacion",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "EstadoCama",
                columns: new[] { "id_estado", "nombre" },
                values: new object[] { 3, "En Mantenimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_creado_por_usuario_id",
                table: "Paciente",
                column: "creado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_Internacion_creado_por_usuario_id",
                table: "Internacion",
                column: "creado_por_usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Internacion_Usuario",
                table: "Internacion",
                column: "creado_por_usuario_id",
                principalTable: "Usuario",
                principalColumn: "usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Paciente_Usuario",
                table: "Paciente",
                column: "creado_por_usuario_id",
                principalTable: "Usuario",
                principalColumn: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Internacion_Usuario",
                table: "Internacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_Usuario",
                table: "Paciente");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_creado_por_usuario_id",
                table: "Paciente");

            migrationBuilder.DropIndex(
                name: "IX_Internacion_creado_por_usuario_id",
                table: "Internacion");

            migrationBuilder.DeleteData(
                table: "EstadoCama",
                keyColumn: "id_estado",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "creado_por_usuario_id",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "creado_por_usuario_id",
                table: "Internacion");
        }
    }
}
