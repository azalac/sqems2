
/**
 * Author:  austin-z
 * Created: Jan 9, 2019
 */

USE master
GO

-- Force closes all existing connections to the database before dropping it
IF EXISTS(select * from sys.databases where name='EMS2')
ALTER DATABASE [EMS2] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO

DROP DATABASE IF EXISTS [EMS2]
GO

CREATE DATABASE [EMS2]
GO

USE [EMS2]
GO

CREATE TABLE [Sexes] (
	ID INT IDENTITY PRIMARY KEY NOT NULL,
	[Name] CHAR(1)
)
GO

INSERT INTO Sexes ([Name]) VALUES ('M'), ('F'), ('I'), ('H')
GO

CREATE TABLE [HCVStatus] (
	ID INT IDENTITY PRIMARY KEY,
	CodeName CHAR(5),
	FullName VARCHAR(256),
	IsError BIT
)
GO

INSERT INTO [HCVStatus] ([CodeName], [FullName], [IsError]) VALUES
	('NOHCV', 'No previous validation', 0),
	('VALID', 'Valid', 0),
	('VCODE', 'Version code mismatch', 1),
	('PUNKO', 'HCN not found', 1)
GO

-- Represents a person's contact information
CREATE TABLE [Household] (
    ID INT IDENTITY NOT NULL PRIMARY KEY,
    addressLine1 VARCHAR(40),
    addressLine2 VARCHAR(40),
    city VARCHAR(40),
    province VARCHAR(40),
    numPhone VARCHAR(20)
)
GO

-- Represents a person, with no relation to their caregiver/patient status
CREATE TABLE [People] (
    PersonID INT IDENTITY PRIMARY KEY NOT NULL,
    HCN CHAR(12) NOT NULL,
    lastName VARCHAR(40),
    firstName VARCHAR(40),
    mInitial CHAR(1),
    dateBirth DATE,
    sex INT,
	HouseID INT,
	HCVStatusID INT,

	FOREIGN KEY (sex) REFERENCES Sexes(ID),

	FOREIGN KEY (HouseID) REFERENCES Household(ID),

	FOREIGN KEY (HCVStatusID) REFERENCES HCVStatus(ID)
)
GO

-- Represents a person who lives in a house
CREATE TABLE [HouseHead] (
	-- Only one head of house per house, so houseid is primary key
	HouseID INT PRIMARY KEY,
	PersonID INT,
	
    FOREIGN KEY(PersonID)
        REFERENCES People(PersonID)
        ON DELETE CASCADE,
		
    FOREIGN KEY(HouseID)
        REFERENCES Household(ID)
        ON DELETE CASCADE
)
GO

-- Represents a single appointment
CREATE TABLE [Appointment] (
    ID INT IDENTITY NOT NULL PRIMARY KEY,
    [Year] INT NOT NULL,
    [Month] INT NOT NULL,
    [Day] INT NOT NULL,
    TimeSlot INT NOT NULL
)
GO

-- Represents a person going to a single appointment
CREATE TABLE [AppointmentPatient] (
	ID INT NOT NULL IDENTITY PRIMARY KEY,
    AppointmentID INT NOT NULL FOREIGN KEY REFERENCES Appointment(ID),
    PersonID INT NOT NULL FOREIGN KEY REFERENCES People(PersonID),

    -- Whether this person is a patient or a caregiver W.R.T. this appointment
    [IsCaregiver] BIT
)
GO

-- All valid billing codes for a billable procedure
CREATE TABLE [MasterBillingCode] (
	ID INT NOT NULL PRIMARY KEY,
	code CHAR(4),
	StartDate DATE,
	Price FLOAT
)
GO

CREATE TABLE [ProcedureState] (
	ID INT PRIMARY KEY IDENTITY,
	[Code] CHAR(4),
	[FullName] VARCHAR(256),
	IsError BIT
)
GO

INSERT INTO [ProcedureState] ([Code], [FullName], [IsError]) VALUES
	('NONE', 'No response', 0),
	('PAID', 'Paid', 0),
	('DECL', 'Declined', 1),
	('FHCV', 'Invalid HCN', 1),
	('CMOH', 'Contact MoH', 1)
GO

-- Represents a billable procedure (appointment can have many)
CREATE TABLE [BillableProcedure] (
    AppointmentPatientID INT NOT NULL,
    CodeID INT NOT NULL,
	[Status] INT NULL,

    PRIMARY KEY(AppointmentPatientID, CodeID),

    FOREIGN KEY(AppointmentPatientID)
        REFERENCES AppointmentPatient(ID),

    FOREIGN KEY(CodeID)
        REFERENCES MasterBillingCode(ID)
)
GO

CREATE VIEW [CaregiverInfo]
AS SELECT Appointment.ID as AptID, Caregiver.PersonID as CaregiverID, CaregiverInfo.HCN as CaregiverHCN,
     CONCAT(CaregiverInfo.firstName, ' ', CaregiverInfo.mInitial, ' ', CaregiverInfo.lastName) as CaregiverName
    FROM Appointment
    INNER JOIN AppointmentPatient as Caregiver
    ON Caregiver.AppointmentID = Appointment.ID
    INNER JOIN People as CaregiverInfo
    ON CaregiverInfo.PersonID = Caregiver.PersonID
	WHERE Caregiver.IsCaregiver = 1
GO

-- Gathers all one-to-one information for an appointment
-- Billable procedures must be gathered separately (see BillingInfo View)
CREATE VIEW [AppointmentInfo]
AS SELECT Appointment.ID,

	 Patient.PersonID as PatientID, PatientInfo.HCN as PatientHCN,
     CONCAT(PatientInfo.firstName, ' ', PatientInfo.mInitial, ' ', PatientInfo.lastName) as PatientName,

	 CaregiverInfo.CaregiverHCN, CaregiverInfo.CaregiverID,
	 CaregiverInfo.CaregiverName,

	 appointment.Year, appointment.Month, appointment.Day, appointment.TimeSlot

    FROM Appointment
    INNER JOIN AppointmentPatient as Patient
    ON Patient.AppointmentID = Appointment.ID
    INNER JOIN People as PatientInfo
    ON PatientInfo.PersonID = Patient.PersonID
	LEFT JOIN CaregiverInfo
	ON CaregiverInfo.AptID = Appointment.ID
    WHERE Patient.IsCaregiver = 0
GO

-- Gathers billing information for one procedure
CREATE VIEW [BillingInfo]
AS SELECT BillableProcedure.AppointmentPatientID, BillableProcedure.CodeID,
     MasterBillingCode.Code, MasterBillingCode.Price, BillableProcedure.[Status]
    FROM BillableProcedure
    INNER JOIN MasterBillingCode
    ON MasterBillingCode.ID = BillableProcedure.CodeID
GO

-- Creates an appointment (DOES NO ERROR CHECKING)
-- Parameters: Year, Month, Day, Timeslot, PatientID, CaregiverID (optional)
CREATE PROCEDURE CreateAppointment (
    @Year INT,
    @Month INT,
    @Day INT,
    @Timeslot INT,
    @PatientID INT,
    @CaregiverID INT = NULL
)
AS BEGIN
	
    DECLARE @aptid INT

    INSERT INTO Appointment ([Year], [Month], [Day], TimeSlot) VALUES (@Year, @Month, @Day, @Timeslot)

	SET @aptid = SCOPE_IDENTITY()

	INSERT INTO AppointmentPatient (AppointmentID, PersonID, IsCaregiver) VALUES (@aptid, @PatientID, 0)

    IF @CaregiverID IS NOT NULL
	BEGIN
		INSERT INTO AppointmentPatient (AppointmentID, PersonID, IsCaregiver) VALUES (@aptid, @CaregiverID, 1)
	END

	SELECT @aptid as 'AppointmentID'

END
GO


CREATE TYPE [dbo].[BillingCodeList] AS TABLE(
    BillingCodeID [INT] NOT NULL
)
GO

CREATE PROCEDURE [dbo].[SetBillableProcedures]
    @AppointmentPatientID INT,
	@BillingCodeList [BillingCodeList] READONLY
AS
	DECLARE @CurrentCodeID INT
	DECLARE BillingCodeCursor CURSOR FORWARD_ONLY FOR SELECT * FROM @BillingCodeList
BEGIN
    
	DELETE FROM BillableProcedure WHERE AppointmentPatientID = @AppointmentPatientID;

	OPEN BillingCodeCursor;

	FETCH NEXT FROM BillingCodeCursor INTO @CurrentCodeID;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO BillableProcedure (AppointmentPatientID, CodeID) VALUES (@AppointmentPatientID, @CurrentCodeID);
		
		FETCH NEXT FROM BillingCodeCursor INTO @CurrentCodeID;
	END

	CLOSE BillingCodeCursor;
	DEALLOCATE BillingCodeCursor;

END
GO


IF NOT EXISTS (SELECT * FROM master.dbo.syslogins WHERE loginname = 'EMS_User')
BEGIN
	CREATE LOGIN [EMS_User] WITH PASSWORD = 'EMS_User_Password'
END
GO

IF NOT EXISTS (SELECT * FROM master.dbo.syslogins WHERE loginname = 'EMS_User')
BEGIN
	CREATE LOGIN [EMS_User] WITH PASSWORD = 'EMS_User_Password'
END
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'EMS_User')
BEGIN
	CREATE USER [EMS_User] FOR LOGIN [EMS_User]
END
GO

GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE TO [EMS_User]
GO
