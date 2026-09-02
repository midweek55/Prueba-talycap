-- =============================================
-- Script de inicializacion de la base de datos DBClientes.
-- Es idempotente: puede ejecutarse multiples veces sin duplicar objetos ni datos.
-- Es ejecutado automaticamente por ApiClientes.API al iniciar (ver Program.cs).
-- =============================================

-- Crea la tabla Clientes si no existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    CREATE TABLE Clientes (
        IdCliente           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Identificacion      NVARCHAR(50)  NOT NULL,
        Nombre              NVARCHAR(100) NOT NULL,
        Apellido            NVARCHAR(100) NOT NULL,
        Email               NVARCHAR(150) NULL,
        FechaCreacion       DATETIME NOT NULL CONSTRAINT DF_Clientes_FechaCreacion DEFAULT (GETDATE()),
        FechaActualizacion  DATETIME NOT NULL CONSTRAINT DF_Clientes_FechaActualizacion DEFAULT (GETDATE())
    );

    CREATE UNIQUE INDEX UQ_Clientes_Identificacion ON Clientes (Identificacion);
END
GO

-- Crea o actualiza el stored procedure sp_ObtenerClientePorIdentificacion
CREATE OR ALTER PROCEDURE sp_ObtenerClientePorIdentificacion
    @Identificacion NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdCliente,
        Identificacion,
        Nombre,
        Apellido,
        Email,
        FechaCreacion,
        FechaActualizacion
    FROM Clientes
    WHERE Identificacion = @Identificacion;
END
GO

-- Datos de prueba
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Identificacion = '12345678')
BEGIN
    INSERT INTO Clientes (Identificacion, Nombre, Apellido, Email)
    VALUES ('12345678', 'Juan', 'Perez', 'juan.perez@example.com');
END

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Identificacion = '87654321')
BEGIN
    INSERT INTO Clientes (Identificacion, Nombre, Apellido, Email)
    VALUES ('87654321', 'Maria', 'Gonzalez', 'maria.gonzalez@example.com');
END

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Identificacion = '11223344')
BEGIN
    INSERT INTO Clientes (Identificacion, Nombre, Apellido, Email)
    VALUES ('11223344', 'Carlos', 'Ramirez', 'carlos.ramirez@example.com');
END

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Identificacion = '99887766')
BEGIN
    INSERT INTO Clientes (Identificacion, Nombre, Apellido, Email)
    VALUES ('99887766', 'Ana', 'Torres', 'ana.torres@example.com');
END
GO
