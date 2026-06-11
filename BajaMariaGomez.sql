USE clinicks_bd;
GO

DECLARE @Dni INT = 44555666;

DELETE FROM MovimientoCama
WHERE id_internacion IN (
    SELECT id_internacion FROM Internacion WHERE dni = @Dni
);

DELETE FROM Internacion
WHERE dni = @Dni;

DELETE FROM Direccion
WHERE dni = @Dni;

DELETE FROM Paciente
WHERE dni = @Dni;
GO
