use master
go
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'MedicineDB')
BEGIN
    DROP DATABASE MedicineDB;
END
Go
Create Database MedicineDB
Go
USE MedicineDB
 CREATE TABLE Users (
 UserId int NOT NULL IDENTITY(2,1) PRIMARY KEY,
 FirstName NVarchar (255),
 LastName NVarchar (255),
 UserName Varchar(30),
 UserPass Varchar (255),
 UserRank int, 
 CHECK (UserRank=1 OR UserRank = 2)
 )
 CREATE TABLE Pharmacies (
 PharmacyId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
 PharmacyName NVarchar(255),
 Adress Nvarchar,
 Phone int
 )
 CREATE TABLE MedicineStatuses(
 StatusId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
 MStatus Varchar, CHECK(MStatus = 'Approved' OR MStatus = 'Denied' OR MStatus = 'Checking' OR Mstatus=NULL),
 Notes NVarchar
 )
 CREATE TABLE Medicines(
  MedicineId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
  PharmacyId int
  FOREIGN KEY (PharmacyId) REFERENCES Pharmacies(PharmacyId),
  MedicineName Varchar,
  BrandName Nvarchar,
  StatusId int
  FOREIGN KEY (StatusId) REFERENCES MedicineStatuses(StatusId),
  UserId int
  FOREIGN KEY (UserId) REFERENCES Users(UserId)
 )
 CREATE TABLE Orders(
 OrderId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
 MedicineId int
 FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId),
 UserId int
 FOREIGN KEY (UserId) REFERENCES Users(UserId),
 Receiver Nvarchar,
 Sender Nvarchar
 )

CREATE LOGIN [MedicineAdminLogin] WITH PASSWORD = 'qwerty';
Go
CREATE USER [MedicineAdminUser] FOR LOGIN [MedicineAdminLogin];
Go
ALTER ROLE db_owner ADD MEMBER [MedicineAdminUser];
Go
 

--scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=medicineDB;User ID=MedicineAdminLogin;Password=qwerty;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context MedicineDbContext -DataAnnotations -force

