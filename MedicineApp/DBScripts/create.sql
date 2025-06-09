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
    UserRank INT CHECK (UserRank IN (1, 2, 3)),
    Active BIT NOT NULL
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
    MStatus VARCHAR(20) CHECK (MStatus IN ('Approved', 'Denied', 'Checking', 'Ordered') OR MStatus IS NULL),
    Notes NVARCHAR(500)
);

CREATE TABLE Medicines (
    MedicineId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PharmacyId INT NOT NULL,
    MedicineName VARCHAR(100) NOT NULL,
    BrandName NVARCHAR(100) NOT NULL,
    NeedsPrescription bit NOT NULL,
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
    OStatus NVARCHAR(255) CHECK (OStatus IN ('Approved', 'Denied', 'Pending') OR OStatus IS NULL),
    PrescriptionImage NVARCHAR(255),
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

INSERT INTO Users (Email, FirstName, LastName, UserName, UserPass, UserRank, Active)
VALUES ('Admin@admin.com', 'Admin', 'Admin', 'Admin', 'qwerty', 1, 1);
INSERT INTO Users (Email, FirstName, LastName, UserName, UserPass, UserRank, Active)
VALUES ('Pharma@Pharma.com', 'Pharma', 'Pharma', 'Pharma', 'qwerty', 2, 1);
INSERT INTO Pharmacies (PharmacyName, Adress, Phone, UserId)
VALUES 
('Good Health Pharmacy', '123 Main St, City', '1234567890', 3);
INSERT INTO MedicineStatuses (MStatus, Notes)
VALUES ('Checking', 'Ready for distribution');
INSERT INTO MedicineStatuses(MStatus, Notes)
VALUES('Approved', 'yuh');
INSERT INTO MedicineStatuses (MStatus, Notes)
VALUES ('Denied', '');
INSERT INTO MedicineStatuses (MStatus, Notes)
VALUES ('Ordered', 'Ordered');
INSERT INTO Medicines (PharmacyId, MedicineName, BrandName, StatusId, UserId, NeedsPrescription)
VALUES 
(1, 'Paracetamol', 'Tylenol', 1, 2, 0); 
INSERT INTO Medicines (PharmacyId, MedicineName, BrandName, StatusId, UserId, NeedsPrescription)
VALUES 
(1, 'Septol', 'Septol', 2, 2, 1);
INSERT INTO Orders (MedicineId, UserId, OStatus)
VALUES 
(1, 2, 'Approved'); 
INSERT INTO Medicines (PharmacyId, MedicineName, BrandName, StatusId, UserId, NeedsPrescription)
VALUES 
(1, 'Ibuprofen', 'Advil', 3, 2, 1);

INSERT INTO Medicines (PharmacyId, MedicineName, BrandName, StatusId, UserId, NeedsPrescription)
VALUES 
(1, 'Loratadine', 'Claritin', 4, 2, 0);

SELECT * FROM Users;
SELECT * FROM Pharmacies;
SELECT * FROM MedicineStatuses;
SELECT * FROM Medicines;
SELECT * FROM Orders;
--scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=medicineDB;User ID=MedicineAdminLogin;Password=qwerty;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context MedicineDbContext -DataAnnotations -force

--INSERT INTO [Users] ([Email], [FirstName], [LastName], [UserName], [UserPass], [UserRank])
--VALUES ('test@example.com', 'Test', 'User', 'testuser', 'password123', 2);
