using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSPActualizarPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$s6C3b/e0jIB988uFWKwJruxrPvHViFqdJJdvPjLF2gy0LmphVAr6u");

            var sp = @"
                CREATE PROCEDURE sp_ActualizarPaciente
                    @Dni INT,
                    @Nombre VARCHAR(50),
                    @Apellido VARCHAR(50),
                    @Telefono VARCHAR(30),
                    @Activo BIT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Paciente
                    SET nombre = @Nombre,
                        apellido = @Apellido,
                        telefono = @Telefono,
                        Activo = @Activo
                    WHERE dni = @Dni;
                END";
            migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ActualizarPaciente");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$lDSk0KbM8UhQrtZgcaPVk.XW4QMsVczmdN27g32arYSTZrJRcWzlK");
        }
    }
}
