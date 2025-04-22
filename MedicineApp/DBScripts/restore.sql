
USE master;
GO

-- Declare the database name
DECLARE @DatabaseName NVARCHAR(255) = 'MedicineDB';

-- Generate and execute the kill commands for all active connections
DECLARE @KillCommand NVARCHAR(MAX);

SET @KillCommand = (
    SELECT STRING_AGG('KILL ' + CAST(session_id AS NVARCHAR), '; ')
    FROM sys.dm_exec_sessions
    WHERE database_id = DB_ID(@DatabaseName)
);

IF @KillCommand IS NOT NULL
BEGIN
    EXEC sp_executesql @KillCommand;
    PRINT 'All connections to the database have been terminated.';
END
ELSE
BEGIN
    PRINT 'No active connections to the database.';
END
Go

IF EXISTS (SELECT * FROM sys.databases WHERE name = N'MedicineDB')
BEGIN
    DROP DATABASE MedicineDB;
END
Go

IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'MedicineAdminLogin')
BEGIN
    CREATE LOGIN [MedicineAdminLogin] WITH PASSWORD = 'qwerty';
END
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'MedicineAdminUser')
BEGIN
    CREATE USER [MedicineAdminUser] FOR LOGIN [MedicineAdminLogin];
END
GO
ALTER SERVER ROLE sysadmin ADD MEMBER [MedicineAdminLogin];
Go

ALTER ROLE db_owner ADD MEMBER [MedicineAdminUser];
GO


CREATE Database MedicineDB;
Go
