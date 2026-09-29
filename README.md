# PruebaTecnicaJAFP - Version 1

API REST para el CRUD de clientes construida en capas: `Api` (presentacion), `Business` (reglas y contratos) y `Data` (persistencia SQL Server mediante ADO.NET).

## Requisitos

- .NET 8 SDK.
- SQL Server 2019 o superior.
- Restaurar `../Backup_Prueba_desarrollador.bak` en una instancia de SQL Server accesible.

## Configuracion

1. Restaure el backup como `PruebaTecnicaJAFP`. El backup es la linea base que aporta los catalogos; no se usa como mecanismo de actualizacion entre versiones.
2. Ejecute las migraciones de `database/migrations/` en orden de version. Para V1 ejecute `V1.0.0__create_cliente.sql`; este crea `Cliente`, sus relaciones reales hacia los catalogos del backup y registra la version aplicada en `dbo.SchemaVersion`.
3. Copie `src/PruebaTecnicaJAFP.Api/appsettings.Development.json.example` a `appsettings.Development.json` y complete la cadena de conexion. Este ultimo archivo no se versiona.
4. Restaure, compile y ejecute:

```bash
dotnet restore
dotnet build
dotnet run --project src/PruebaTecnicaJAFP.Api
```

Abra Swagger en la URL que muestre la consola, normalmente `http://localhost:5098/swagger`.

## Endpoints V1

- `GET /api/clientes`
- `GET /api/clientes/{id}`
- `POST /api/clientes`
- `PUT /api/clientes/{id}`
- `GET /api/ubicaciones/paises`
- `GET /api/ubicaciones/paises/{paisCodigo}/departamentos`
- `GET /api/ubicaciones/departamentos/{departamentoCodigo}/ciudades`

La API valida que el tipo de identificacion exista, que la identificacion no este repetida y que la ciudad pertenezca al departamento y al pais seleccionados.

## Decisiones tecnicas

- **Arquitectura en capas:** la API solo atiende HTTP; las reglas viven en Business y el SQL queda encapsulado en Data.
- **ADO.NET:** permite consultas parametrizadas y dependencias minimas. Todas las operaciones usan parametros para evitar inyeccion SQL.
- **SQL Server y migraciones:** la migracion V1.0.0 define claves foraneas, restricciones e indice unico para la identificacion. Toda modificacion futura se entrega como un nuevo script inmutable y versionado, registrado en `dbo.SchemaVersion`.

La interfaz web con Bootstrap, pruebas automatizadas y exportacion Excel se planifican para versiones posteriores.
