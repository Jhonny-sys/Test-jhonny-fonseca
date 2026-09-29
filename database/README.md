# Migraciones de base de datos

Cada archivo de `migrations/` es una migracion **inmutable**, completa y versionada. Se aplica una vez y queda registrada en `dbo.SchemaVersion`.

## Convencion

`V<version>__<descripcion>.sql`, por ejemplo: `V1.1.0__add_cliente_email.sql`.

Una modificacion de estructura, datos de referencia, llaves, indices, restricciones, procedimientos, funciones o triggers exige una nueva migracion. Nunca se modifica una migracion aplicada y nunca se usa un backup como mecanismo de actualizacion.

## Aplicacion por ambiente

1. Restaure `Backup_Prueba_desarrollador.bak` como `PruebaTecnicaJAFP`; esa es la linea base de los catalogos requeridos por la prueba.
2. Revise `dbo.SchemaVersion` y aplique, en orden ascendente, solo los scripts cuya version no figure en esa tabla.
3. Ejecute cada archivo con una cuenta con permisos DDL; cada migracion debe ser transaccional cuando SQL Server lo permita.
4. Verifique el registro en `dbo.SchemaVersion` tras la ejecucion.

Ejemplo con sqlcmd:

```bash
sqlcmd -S SERVIDOR -d PruebaTecnicaJAFP -E -i migrations/V1.0.0__create_cliente.sql
```

Para produccion, la ejecucion debe formar parte del pipeline de despliegue y estar aprobada antes de aplicar cambios.
