/*
  PruebaTecnicaJAFP - Migracion V1.0.0
  Linea base requerida: Backup_Prueba_desarrollador.bak restaurado como PruebaTecnicaJAFP.
  Descripcion: crea Cliente y sus relaciones con los catalogos existentes del backup.
  Esta migracion es inmutable y solo se ejecuta una vez por ambiente.
*/
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.Pais', N'U') IS NULL
   OR OBJECT_ID(N'dbo.DepartamentosColombia', N'U') IS NULL
   OR OBJECT_ID(N'dbo.DivisionPoliticaColombia', N'U') IS NULL
    THROW 51000, 'No existe la linea base del backup requerida para aplicar V1.0.0.', 1;
GO

IF OBJECT_ID(N'dbo.SchemaVersion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SchemaVersion (
        Version NVARCHAR(50) NOT NULL CONSTRAINT PK_SchemaVersion PRIMARY KEY,
        Descripcion NVARCHAR(250) NOT NULL,
        AplicadaEnUtc DATETIME2(0) NOT NULL CONSTRAINT DF_SchemaVersion_AplicadaEnUtc DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF EXISTS (SELECT 1 FROM dbo.SchemaVersion WHERE Version = N'1.0.0')
    THROW 51001, 'La migracion 1.0.0 ya fue aplicada. No se puede ejecutar nuevamente.', 1;

IF OBJECT_ID(N'dbo.Cliente', N'U') IS NOT NULL
    THROW 51002, 'La tabla Cliente ya existe y no esta registrada como migracion V1.0.0.', 1;
GO

BEGIN TRANSACTION;

CREATE TABLE dbo.Cliente (
    ClnId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cliente PRIMARY KEY,
    ClnTipoId SMALLINT NOT NULL,
    ClnNumeroIdentificacion VARCHAR(30) NOT NULL,
    ClnRazonSocial VARCHAR(150) NOT NULL,
    ClnPaisCodigo SMALLINT NOT NULL,
    ClnDptColCodigoDane INT NOT NULL,
    ClnDvsPltColCodigoDane INT NOT NULL,
    CONSTRAINT FK_Cliente_Pais
        FOREIGN KEY (ClnPaisCodigo) REFERENCES dbo.Pais(PaisCodigo),
    CONSTRAINT FK_Cliente_Departamento
        FOREIGN KEY (ClnDptColCodigoDane) REFERENCES dbo.DepartamentosColombia(DptColCodigoDane),
    CONSTRAINT FK_Cliente_DivisionPolitica
        FOREIGN KEY (ClnDvsPltColCodigoDane) REFERENCES dbo.DivisionPoliticaColombia(DvsPltColCodigoDane),
    CONSTRAINT UQ_Cliente_NumeroIdentificacion UNIQUE (ClnNumeroIdentificacion)
);

CREATE INDEX IX_Cliente_Ubicacion
    ON dbo.Cliente (ClnPaisCodigo, ClnDptColCodigoDane, ClnDvsPltColCodigoDane);

INSERT INTO dbo.SchemaVersion (Version, Descripcion)
VALUES (N'1.0.0', N'Crea Cliente relacionado con los catalogos del backup');

COMMIT TRANSACTION;
GO
