RESTORE DATABASE [PruebaTecnicaJAFP]
FROM DISK = '/var/opt/mssql/backup/Backup_Prueba_desarrollador.bak'
WITH
    MOVE N'Prueba'
        TO N'/var/opt/mssql/data/PruebaTecnicaJAFP.mdf',
    MOVE N'Prueba_log'
        TO N'/var/opt/mssql/data/PruebaTecnicaJAFP_log.ldf',
    RECOVERY,
    STATS = 10;