USE [EMS2]
GO

BEGIN

DECLARE @patient INT
DECLARE @caregiver INT
DECLARE @apt1id INT
DECLARE @TestCodes BillingCodeList
DECLARE @house INT

INSERT INTO people(HCN, lastName, firstName, mInitial, dateBirth, sex, HouseID) VALUES
	('0123456789AB', 'smith', 'bob', 'c', '1997-11-20', (SELECT ID FROM Sexes WHERE [Name] = 'M'), NULL),
	('0123456789AB', 'smithert', 'bobert', 'c', '1997-11-21', (SELECT ID FROM Sexes WHERE [Name] = 'M'), NULL);

SELECT @patient = (SELECT PersonID FROM People WHERE firstName = 'bob'),
	   @caregiver = (SELECT PersonID FROM People WHERE firstName = 'bobert');

INSERT INTO Household(addressLine1, city, province, numPhone) VALUES ('123 yes st', 'guelph', 'ON', '123-123-1234');
SELECT @house = SCOPE_IDENTITY();

INSERT INTO HouseHead (HouseID, PersonID) VALUES (@house, @caregiver);

UPDATE People SET HouseID = @house;

EXEC dbo.CreateAppointment 2005, 5, 25, 3, @patient, @caregiver;
EXEC dbo.CreateAppointment 2005, 5, 25, 4, @patient, NULL;

SELECT TOP(1) @apt1id = AppointmentID FROM AppointmentPatient WHERE PersonID = @patient;

INSERT INTO @TestCodes VALUES (1), (2), (3);

EXEC dbo.SetBillableProcedures @apt1id, @TestCodes;

END

