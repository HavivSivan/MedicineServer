CREATE DATABASE medicine_server
 CREATE TABLE Users (
 UserId int NOT NULL PRIMARY KEY,
 FirstName NVarchar (255),
 LastName NVarchar (255),
 UserName Varchar(30),
 UserPass Varchar (255),
 UserRank int CHECK(UserRank=1 OR UserRank = 2)
 );
 CREATE TABLE Orders(
 OrderId int NOT NULL PRIMARY KEY,
 FOREIGN KEY (FK_Medicine) REFERENCES Medicines(MedicineId),
 FOREIGN KEY (FK_Receiver) REFERENCES Users(UserId),
 Receiver Geography,
 Sender Geography,
 );
 CREATE TABLE Medicines(
  MedicineId int NOT NULL PRIMARY KEY,
  FOREIGN KEY (FK_PharmacyId) REFERENCES Pharmacies(PharmacyId),
  MedicineName Varchar,
  BrandName Nvarchar,
  FOREIGN KEY (FK_StatusId) REFERENCES MedicineStatuses(StatusId),
 );
 CREATE TABLE MedicineStatuses(
 StatusId int NOT NULL PRIMARY KEY,
 MStatus Varchar CHECK(MStatus = "Approved" OR MStatus = "Denied" OR MStatus = "Checking"),
 Notes NVarchar,
 );
 CREATE TABLE Pharmacies (
 PharmacyId int NOT NULL PRIMARY KEY,
 PharmacyName NVarchar(255),
 Adress Geography,
 Phone int,
 );
