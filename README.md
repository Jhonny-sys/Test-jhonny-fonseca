# PruebaTecnicaJAFP - Version 1

API REST para el CRUD de clientes construida en capas: `Api` (presentacion), `Business` (reglas y contratos) y `Data` (persistencia SQL Server mediante ADO.NET).

## Requisitos

- .NET 8 SDK.
- SQL Server 2019 o superior.
- Restaurar `../Backup_Prueba_desarrollador.bak` en una instancia de SQL Server accesible.

## Configuracion

1. Restaure el backup como `PruebaTecnicaJAFP`. El backup es la linea base que aporta los catalogos; no se usa como mecanismo de actualizacion entre versiones.
2. Ejecute las migraciones de `database/migrations/` en orden de version: `V1.0.0__create_cliente.sql` crea `Cliente` con sus relaciones hacia los catalogos del backup, y `V1.1.0__create_procedimientos_almacenados.sql` crea los procedimientos almacenados CRUD (`usp_Cliente_Crear`, `usp_Cliente_Actualizar`, `usp_Cliente_ConsultarTodos`, `usp_Cliente_ConsultarPorId`, `usp_Cliente_ExisteIdentificacion`). Cada migracion queda registrada en `dbo.SchemaVersion`.
3. Copie `src/PruebaTecnicaJAFP.Api/appsettings.Development.json.example` a `appsettings.Development.json` y complete la cadena de conexion. Este ultimo archivo no se versiona.
4. Restaure, compile y ejecute:

```bash
dotnet restore
dotnet build
dotnet run --project src/PruebaTecnicaJAFP.Api
```

Abra Swagger en la URL que muestre la consola, normalmente `http://localhost:5098/swagger` y la interfaz visual en la URL que se muestra `http://localhost:5098` y las peticiones api sobre la siguiente URL `http://localhost:5098/api/`.

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
- **Procedimientos almacenados:** las operaciones CRUD de clientes se ejecutan mediante procedimientos almacenados (`usp_Cliente_*`), llamados con parametros desde ADO.NET; las validaciones de negocio (duplicados, ubicacion valida) siguen en Business y se refuerzan en la base de datos.
- **SQL Server y migraciones:** V1.0.0 define la tabla Cliente con claves foraneas, restricciones e indice unico; V1.1.0 agrega los procedimientos almacenados. Toda modificacion futura se entrega como un nuevo script inmutable y versionado, registrado en `dbo.SchemaVersion`.

La interfaz web con Bootstrap, pruebas automatizadas y exportacion Excel se planifican para versiones posteriores.
