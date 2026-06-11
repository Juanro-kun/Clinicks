USE clinicks_bd;
GO

-- 1. Identificar pacientes a borrar (sin telefono o Juan Perez)
SELECT dni INTO #PacientesABorrar
FROM Paciente
WHERE (telefono IS NULL OR telefono = '') 
   OR (LOWER(nombre) = 'juan' AND LOWER(apellido) = 'pérez')
   OR (LOWER(nombre) = 'juan' AND LOWER(apellido) = 'perez');

-- 2. Eliminar MovimientosCama de las internaciones de esos pacientes
DELETE FROM MovimientoCama
WHERE id_internacion IN (
    SELECT id_internacion FROM Internacion WHERE dni IN (SELECT dni FROM #PacientesABorrar)
);

-- 3. Eliminar Internaciones
DELETE FROM Internacion
WHERE dni IN (SELECT dni FROM #PacientesABorrar);

-- 4. Eliminar Direcciones
DELETE FROM Direccion
WHERE dni IN (SELECT dni FROM #PacientesABorrar);

-- 5. Finalmente, eliminar los pacientes (Baja física)
DELETE FROM Paciente
WHERE dni IN (SELECT dni FROM #PacientesABorrar);

DROP TABLE #PacientesABorrar;
GO
