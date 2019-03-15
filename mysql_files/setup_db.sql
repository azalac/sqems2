
/**
 * Author:  austin-z
 * Created: Jan 9, 2019
 */

-- If you want to run this file, update the path at the bottom (in the load data query)

DROP SCHEMA IF EXISTS `EMS2`;

CREATE SCHEMA `EMS2`;

USE `EMS2`;

-- drops all tables, regardless of their foreign keys
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `People`;
DROP TABLE IF EXISTS `Household`;

DROP TABLE IF EXISTS `Appointment`;
DROP TABLE IF EXISTS `AppointmentPatient`;

DROP TABLE IF EXISTS `BillableProcedure`;

DROP VIEW IF EXISTS `AppointmentInfo`;

DROP VIEW IF EXISTS `BillingInfo`;

SET FOREIGN_KEY_CHECKS = 1;

-- Represents a person, with no relation to their caregiver/patient status
CREATE TABLE `People` (
    PersonID INT AUTO_INCREMENT NOT NULL,
    HCN CHAR(12) NOT NULL,
    lastName VARCHAR(40),
    firstName VARCHAR(40),
    mInitial CHAR(1),
    dateBirth DATE,
    sex ENUM('M', 'F', 'I', 'H'),
    HouseID INT,

    PRIMARY KEY(PersonID)
) ENGINE=INNODB;

-- Represents a person's contact information
CREATE TABLE `Household` (
    ID INT AUTO_INCREMENT NOT NULL,
    addressLine1 VARCHAR(40),
    addressLine2 VARCHAR(40),
    city VARCHAR(40),
    province VARCHAR(40), -- TODO: make this an enum
    numPhone VARCHAR(20),
    headOfHouse INT NOT NULL,

    PRIMARY KEY(ID),
    FOREIGN KEY(headOfHouse)
        REFERENCES People(PersonID)
        ON DELETE RESTRICT
        ON UPDATE NO ACTION
) ENGINE=INNODB;

-- Represents a single appointment
CREATE TABLE `Appointment` (
    ID INT AUTO_INCREMENT NOT NULL,
    `Year` INT NOT NULL,
    `Month` INT NOT NULL,
    `Day` INT NOT NULL,
    TimeSlot INT NOT NULL,

    PRIMARY KEY(ID)
);

-- Represents a person going to a single appointment
CREATE TABLE `AppointmentPatient` (
    AppointmentID INT NOT NULL,
    PersonID INT NOT NULL,

    -- Whether this person is a patient or a caregiver W.R.T. this appointment
    `Type` ENUM('Patient', 'Caregiver'),

    PRIMARY KEY(AppointmentID, PersonID),

    FOREIGN KEY(AppointmentID)
        REFERENCES Appointment(ID),

    FOREIGN KEY(PersonID)
        REFERENCES People(PersonID)
);

-- All valid billing codes for a billable procedure
CREATE TABLE `MasterBillingCode` (
    ID INT NOT NULL,
    Code CHAR(4),
    StartDate DATE,
    Price DOUBLE,

    PRIMARY KEY(ID)
);

-- Represents a billable procedure (appointment can have many)
CREATE TABLE `BillableProcedure` (
    AppointmentID INT NOT NULL,
    PersonID INT NOT NULL,
    CodeID INT NOT NULL,

    PRIMARY KEY(AppointmentID, PersonID, CodeID),

    FOREIGN KEY(AppointmentID)
        REFERENCES AppointmentPatient(AppointmentID),

    FOREIGN KEY(PersonID)
        REFERENCES AppointmentPatient(PersonID),

    FOREIGN KEY(CodeID)
        REFERENCES MasterBillingCode(ID)
);

-- Gathers all one-to-one information for an appointment
-- Billable procedures must be gathered separately (see BillingInfo View)
CREATE VIEW `AppointmentInfo`
AS SELECT Appointment.ID, Patient.PersonID as PatientID, Caregiver.PersonID as CaregiverID,
     CONCAT(PatientInfo.firstName, ' ', PatientInfo.mInitial, ' ', PatientInfo.lastName) as PatientName,
     CONCAT(CaregiverInfo.firstName, ' ', CaregiverInfo.mInitial, ' ', CaregiverInfo.lastName) as CaregiverName,
     PatientInfo.HCN as PatientHCN, CaregiverInfo.HCN as CaregiverHCN,
	 appointment.Year, appointment.Month, appointment.Day, appointment.TimeSlot
    FROM Appointment
    INNER JOIN AppointmentPatient as Patient
    ON Patient.AppointmentID = Appointment.ID
    INNER JOIN AppointmentPatient as Caregiver
    ON Caregiver.AppointmentID = Appointment.ID
    INNER JOIN People as PatientInfo
    ON PatientInfo.PersonID = Patient.PersonID
    INNER JOIN People as CaregiverInfo
    ON CaregiverInfo.PersonID = Caregiver.PersonID
    WHERE Patient.`Type` = 'Patient' and Caregiver.`Type` = 'Caregiver';

-- Gathers billing information for one procedure
CREATE VIEW `BillingInfo`
AS SELECT BillableProcedure.AppointmentID, BillableProcedure.PersonID, BillableProcedure.CodeID,
     MasterBillingCode.Code, MasterBillingCode.Price
    FROM BillableProcedure
    INNER JOIN MasterBillingCode
    ON MasterBillingCode.ID = BillableProcedure.CodeID;

-- Creates an appointment (DOES NO ERROR CHECKING)
-- Parameters: Year, Month, Day, Timeslot, PatientID, CaregiverID (optional)
DELIMITER //
CREATE PROCEDURE CreateAppointment (
    IN Year INT,
    IN Month INT,
    IN Day INT,
    IN Timeslot INT,
    IN PatientID INT,
    IN CaregiverID INT
)
BEGIN
	
    DECLARE aptid INT;
    DECLARE CUSTOM_EXCEPTION CONDITION FOR SQLSTATE '45000';
    
	DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
    END;
    
    IF PatientID IS NULL THEN
        SIGNAL CUSTOM_EXCEPTION
		SET MESSAGE_TEXT = 'PatientID may not be null';
	END IF;
    
    START TRANSACTION;

    INSERT INTO Appointment (`Year`, `Month`, `Day`, `TimeSlot`) VALUES (Year, Month, Day, Timeslot);
    
    SET aptid = LAST_INSERT_ID();
    
	INSERT INTO AppointmentPatient VALUES (aptid, PatientID, 'Patient');
	
    IF CaregiverID IS NOT NULL THEN
		INSERT INTO AppointmentPatient VALUES (aptid, CaregiverID, 'Caregiver');
	END IF;
    
	COMMIT;
    
END //
DELIMITER ;

-- make sure to update this path!
LOAD DATA LOCAL INFILE 'C:/Users/azalac0020/Desktop/ohipfee_sql.txt' INTO TABLE MasterBillingCode;

-- Ignore errors from this line
DROP USER 'account';

CREATE USER 'account';

-- For some reason, mysql doesn't like 'IDENTIFIED BY ...'
SET PASSWORD FOR 'account' = PASSWORD('accountpass');

GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON EMS2.* TO 'account';

FLUSH PRIVILEGES;

-- configure the master billing code table
-- REVOKE ALL PRIVILEGES ON EMS2.MasterBillingCode FROM 'account';