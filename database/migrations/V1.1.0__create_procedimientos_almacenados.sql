/*
  PruebaTecnicaJAFP - Migracion V1.1.0
  Requiere: V1.0.0 aplicada (tablas dbo.Cliente y dbo.SchemaVersion).
  Descripcion: crea los procedimientos almacenados CRUD para dbo.Cliente
               (insertar, editar, consultar todos, consultar por id y
               verificar identificacion duplicada), requeridos por la prueba.
  Esta migracion es inmutable y solo se ejecuta una vez por ambiente.
*/
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Cliente', N'U') IS NULL
    THROW 51000, 'No existe la tabla dbo.Cliente. Aplique V1.0.0 antes de V1.1.0.', 1;
GO

IF OBJECT_ID(N'dbo.SchemaVersion', N'U') IS NULL
    THROW 51000, 'No existe dbo.SchemaVersion. Aplique V1.0.0 antes de V1.1.0.', 1;
GO

IF EXISTS (SELECT 1 FROM dbo.SchemaVersion WHERE Version = N'1.1.0')
    THROW 51001, 'La migracion 1.1.0 ya fue aplicada. No se puede ejecutar nuevamente.', 1;

IF OBJECT_ID(N'dbo.usp_Cliente_Crear', N'P') IS NOT NULL
   OR OBJECT_ID(N'dbo.usp_Cliente_Actualizar', N'P') IS NOT NULL
   OR OBJECT_ID(N'dbo.usp_Cliente_ConsultarTodos', N'P') IS NOT NULL
   OR OBJECT_ID(N'dbo.usp_Cliente_ConsultarPorId', N'P') IS NOT NULL
   OR OBJECT_ID(N'dbo.usp_Cliente_ExisteIdentificacion', N'P') IS NOT NULL
    THROW 51002, 'Existen procedimientos almacenados de V1.1.0 sin registrar en dbo.SchemaVersion.', 1;
GO

BEGIN TRANSACTION;
GO

CREATE PROCEDURE dbo.usp_Cliente_Crear
    @ClnTipoId SMALLINT,
    @ClnNumeroIdentificacion VARCHAR(30),
    @ClnRazonSocial VARCHAR(150),
    @ClnPaisCodigo SMALLINT,
    @ClnDptColCodigoDane INT,
    @ClnDvsPltColCodigoDane INT,
    @ClnId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Cliente WHERE ClnNumeroIdentificacion = @ClnNumeroIdentificacion)
        THROW 50001, 'Ya existe un cliente con esta identificacion.', 1;

    INSERT INTO dbo.Cliente
        (ClnTipoId, ClnNumeroIdentificacion, ClnRazonSocial, ClnPaisCodigo, ClnDptColCodigoDane, ClnDvsPltColCodigoDane)
    VALUES
        (@ClnTipoId, @ClnNumeroIdentificacion, @ClnRazonSocial, @ClnPaisCodigo, @ClnDptColCodigoDane, @ClnDvsPltColCodigoDane);

    SET @ClnId = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.usp_Cliente_Actualizar
    @ClnId INT,
    @ClnTipoId SMALLINT,
    @ClnNumeroIdentificacion VARCHAR(30),
    @ClnRazonSocial VARCHAR(150),
    @ClnPaisCodigo SMALLINT,
    @ClnDptColCodigoDane INT,
    @ClnDvsPltColCodigoDane INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE ClnId = @ClnId)
        RETURN 0;

    IF EXISTS (SELECT 1 FROM dbo.Cliente WHERE ClnNumeroIdentificacion = @ClnNumeroIdentificacion AND ClnId <> @ClnId)
        THROW 50001, 'Ya existe un cliente con esta identificacion.', 1;

    UPDATE dbo.Cliente
    SET ClnTipoId = @ClnTipoId,
        ClnNumeroIdentificacion = @ClnNumeroIdentificacion,
        ClnRazonSocial = @ClnRazonSocial,
        ClnPaisCodigo = @ClnPaisCodigo,
        ClnDptColCodigoDane = @ClnDptColCodigoDane,
        ClnDvsPltColCodigoDane = @ClnDvsPltColCodigoDane
    WHERE ClnId = @ClnId;

    RETURN 1;
END;
GO

CREATE PROCEDURE dbo.usp_Cliente_ConsultarTodos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.ClnId,
           c.ClnTipoId,
           c.ClnNumeroIdentificacion,
           c.ClnRazonSocial,
           c.ClnPaisCodigo,
           c.ClnDptColCodigoDane,
           c.ClnDvsPltColCodigoDane,
           d.DptColNombredelDepartamento,
           m.DvsPltColNombreMunicipio
    FROM dbo.Cliente c
    INNER JOIN dbo.DepartamentosColombia d ON d.DptColCodigoDane = c.ClnDptColCodigoDane
    INNER JOIN dbo.DivisionPoliticaColombia m ON m.DvsPltColCodigoDane = c.ClnDvsPltColCodigoDane
    ORDER BY c.ClnRazonSocial;
END;
GO

CREATE PROCEDURE dbo.usp_Cliente_ConsultarPorId
    @ClnId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.ClnId,
           c.ClnTipoId,
           c.ClnNumeroIdentificacion,
           c.ClnRazonSocial,
           c.ClnPaisCodigo,
           c.ClnDptColCodigoDane,
           c.ClnDvsPltColCodigoDane,
           d.DptColNombredelDepartamento,
           m.DvsPltColNombreMunicipio
    FROM dbo.Cliente c
    INNER JOIN dbo.DepartamentosColombia d ON d.DptColCodigoDane = c.ClnDptColCodigoDane
    INNER JOIN dbo.DivisionPoliticaColombia m ON m.DvsPltColCodigoDane = c.ClnDvsPltColCodigoDane
    WHERE c.ClnId = @ClnId;
END;
GO

CREATE PROCEDURE dbo.usp_Cliente_ExisteIdentificacion
    @ClnNumeroIdentificacion VARCHAR(30),
    @ClnId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IIF(EXISTS (SELECT 1
                       FROM dbo.Cliente
                       WHERE ClnNumeroIdentificacion = @ClnNumeroIdentificacion
                         AND (@ClnId IS NULL OR ClnId <> @ClnId)), 1, 0) AS Existe;
END;
GO

INSERT INTO dbo.SchemaVersion (Version, Descripcion)
VALUES (N'1.1.0', N'Crea los procedimientos almacenados CRUD para Cliente');

COMMIT TRANSACTION;
GO
