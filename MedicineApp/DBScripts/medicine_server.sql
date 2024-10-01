CREATE DATABASE medicine_server
 CREATE TABLE Users (
 UserId int NOT NULL PRIMARY KEY,
 FirstName NVarchar (255),
 LastName NVarchar (255),
 IsAdmin bit,
 );
 CREATE TABLE Orders(
 OrderId int NOT NULL PRIMARY KEY,

 );
 CREATE TABLE Medicines(
  MedicineId int NOT NULL PRIMARY KEY,
  FOREIGN KEY (FK_PharmacyId) REFERENCES Pharmacies(PharmacyId) 
 );
 CREATE TABLE MedicineStatuses(
 FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId),
 );
 CREATE TABLE Pharmacies (
 PharmacyId int NOT NULL PRIMARY KEY,
 PharmacyName NVarchar(255),
 Adress Geography,

 );
