using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinicks.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSPConsultarPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$lDSk0KbM8UhQrtZgcaPVk.XW4QMsVczmdN27g32arYSTZrJRcWzlK");

            var sp = @"
                CREATE PROCEDURE sp_ConsultarPaciente_Count
                    @Dni INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT COUNT(*)
                    FROM Paciente
                    WHERE dni = @Dni;
                END";
            migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ConsultarPaciente_Count");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "usuario_id",
                keyValue: 1,
                column: "password",
                value: "$2a$11$00M769sgsGbVZG3ruLchD.b0fa.74lB5OY0FpC1onP6dpZ25pxzIa");
        }
    }
}
