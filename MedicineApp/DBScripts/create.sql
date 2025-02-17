USE master
GO
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'MedicineDB')
BEGIN
    ALTER DATABASE MedicineDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MedicineDB;
END
GO
CREATE DATABASE MedicineDB;
GO
USE MedicineDB;

CREATE TABLE Users (
    UserId INT NOT NULL IDENTITY(2,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(255) NOT NULL,
    LastName NVARCHAR(255) NOT NULL,
    UserName VARCHAR(30) NOT NULL,
    UserPass VARCHAR(255) NOT NULL,
    UserRank INT CHECK (UserRank IN (1, 2, 3))
);

CREATE TABLE Pharmacies (
    PharmacyId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    PharmacyName NVARCHAR(255) NOT NULL,
    Adress NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(15) NOT NULL,
    UserId INT NOT NULL, 
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE MedicineStatuses (
    StatusId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    MStatus VARCHAR(20) CHECK (MStatus IN ('Approved', 'Denied', 'Checking') OR MStatus IS NULL),
    Notes NVARCHAR(500)
);

CREATE TABLE Medicines (
    MedicineId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PharmacyId INT NOT NULL,
    MedicineName VARCHAR(100) NOT NULL,
    BrandName NVARCHAR(100) NOT NULL,
    StatusId INT NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (PharmacyId) REFERENCES Pharmacies(PharmacyId),
    FOREIGN KEY (StatusId) REFERENCES MedicineStatuses(StatusId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE Orders (
    OrderId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    MedicineId INT NOT NULL,
    UserId INT NOT NULL,
    Receiver NVARCHAR(255) NOT NULL,
    Sender NVARCHAR(255) NOT NULL,
    FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

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


ALTER ROLE db_owner ADD MEMBER [MedicineAdminUser];
GO

INSERT INTO Users (Email, FirstName, LastName, UserName, UserPass, UserRank)
VALUES ('Admin@admin.com', 'Admin', 'Admin', 'Admin', 'qwerty', 1);
INSERT INTO Pharmacies (PharmacyName, Adress, Phone, UserId)
VALUES 
('Good Health Pharmacy', '123 Main St, City', '1234567890', 2);
INSERT INTO MedicineStatuses (MStatus, Notes)
VALUES ('Approved', 'Ready for distribution');
INSERT INTO Medicines (PharmacyId, MedicineName, BrandName, StatusId, UserId)
VALUES 
(1, 'Paracetamol', 'Tylenol', 1, 2); 
INSERT INTO Orders (MedicineId, UserId, Receiver, Sender)
VALUES 
(1, 2, 'Admin Admin', 'Good Health Pharmacy'); 
SELECT * FROM Users;
SELECT * FROM Pharmacies;
SELECT * FROM MedicineStatuses;
SELECT * FROM Medicines;
SELECT * FROM Orders;
--scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=medicineDB;User ID=MedicineAdminLogin;Password=qwerty;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context MedicineDbContext -DataAnnotations -force

--INSERT INTO [Users] ([Email], [FirstName], [LastName], [UserName], [UserPass], [UserRank])
--VALUES ('test@example.com', 'Test', 'User', 'testuser', 'password123', 2);
