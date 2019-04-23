/**
 * Author:  mike-h
 * Created: April 13, 2019
 * Description: This file sets up all roles, users, logins, and associated
 *				permissions for the EMS2 database.
 */



USE [master]

-- create ems server admin role
IF (SELECT COUNT(name) FROM sys.server_principals WHERE name='EMS_Server_Admin_Role') < 1
BEGIN
	CREATE SERVER ROLE [EMS_Server_Admin_Role]
END
GO
GRANT CONTROL SERVER TO [EMS_Server_Admin_Role]
GO 

-- create ems regular user server role
IF (SELECT COUNT(name) FROM sys.server_principals WHERE name='EMS_Server_Role') < 1
BEGIN
	CREATE SERVER ROLE [EMS_Server_Role]
END
GO
REVOKE CONTROL SERVER TO [EMS_Server_Role]
GO 

USE [EMS2]

-- create admin login and add to server admin role
IF (SELECT COUNT(loginname) FROM master.dbo.syslogins WHERE loginname = 'EMS_Admin_Login') < 1
BEGIN
	CREATE LOGIN [EMS_Admin_Login] WITH PASSWORD=N'EMS_Admin_Password', DEFAULT_DATABASE=[EMS2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END
GO
ALTER SERVER ROLE [EMS_Server_Admin_Role] ADD MEMBER [EMS_Admin_Login]
GO

-- create doctor login and add to regular server role
IF (SELECT COUNT(loginname) FROM master.dbo.syslogins WHERE loginname = 'EMS_Doctor_Login') < 1
BEGIN
	CREATE LOGIN [EMS_Doctor_Login] WITH PASSWORD=N'EMS_Doctor_Password', DEFAULT_DATABASE=[EMS2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END
GO
ALTER SERVER ROLE [EMS_Server_Role] ADD MEMBER [EMS_Doctor_Login]
GO

-- create nurse login and add to regular server role
IF (SELECT COUNT(loginname) FROM master.dbo.syslogins WHERE loginname = 'EMS_Nurse_Login') < 1
BEGIN
	CREATE LOGIN [EMS_Nurse_Login] WITH PASSWORD=N'EMS_Nurse_Password', DEFAULT_DATABASE=[EMS2], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END
GO
ALTER SERVER ROLE [EMS_Server_Role] ADD MEMBER [EMS_Nurse_Login]
GO

-- create admin database role
IF (SELECT COUNT(name) FROM sys.database_principals WHERE name='EMS_Admin_Role') < 1
BEGIN
	CREATE ROLE [EMS_Admin_Role]
END
GO

-- give admin full permissions
GRANT CONTROL TO [EMS_Admin_Role]
GO

-- create doctor database role
IF (SELECT COUNT(name) FROM sys.database_principals WHERE name='EMS_Doctor_Role') < 1
BEGIN
	CREATE ROLE [EMS_Doctor_Role]
END
GO

-- grant needed permissions to doctor role
GRANT SELECT TO [EMS_Doctor_Role]
GRANT EXEC TO [EMS_Doctor_Role]
GRANT UPDATE TO [EMS_Doctor_Role]
GRANT INSERT TO [EMS_Doctor_Role]

-- restrict doctor from update or insert on appointment, appointmentpatient and masterbillingcode tables
DENY UPDATE, INSERT ON [dbo].[Appointment] TO [EMS_Doctor_Role]
GO
DENY UPDATE, INSERT ON [dbo].[AppointmentPatient] TO [EMS_Doctor_Role]
GO
DENY UPDATE, INSERT ON [dbo].[MasterBillingCode] TO [EMS_Doctor_Role]
GO

-- restrict delete, alter, create and drop to doctor role
DENY DELETE TO [EMS_Doctor_Role]
GO
DENY ALTER TO [EMS_Doctor_Role]
GO

-- create nurse database role
IF (SELECT COUNT(name) FROM sys.database_principals WHERE name='EMS_Nurse_Role') < 1
BEGIN
	CREATE ROLE [EMS_Nurse_Role]
END
GO

-- grant needed permissions to nurse role
GRANT SELECT TO [EMS_Nurse_Role]
GRANT EXEC TO [EMS_Nurse_Role]
GRANT UPDATE TO [EMS_Nurse_Role]
GRANT INSERT TO [EMS_Nurse_Role]

-- restrict nurse from update or insert on billable procedures and masterbillingcode tables
DENY UPDATE, INSERT ON [dbo].[BillableProcedure] TO [EMS_Nurse_Role]
GO
DENY UPDATE, INSERT ON [dbo].[MasterBillingCode] TO [EMS_Nurse_Role]
GO

-- restrict delete, alter, create and drop to nurse role
DENY DELETE TO [EMS_Nurse_Role]
GO
DENY ALTER TO [EMS_Nurse_Role]
GO

-- create admin user for login
IF (SELECT COUNT(name) FROM sys.sysusers WHERE name = N'EMS_Admin_User') < 1
BEGIN
	CREATE USER [EMS_Admin_User] FOR LOGIN [EMS_Admin_Login] WITH DEFAULT_SCHEMA=[dbo]
END
GO

-- create doctor user for login
IF (SELECT COUNT(name) FROM sys.sysusers WHERE name = N'EMS_Doctor_User') < 1
BEGIN
	CREATE USER [EMS_Doctor_User] FOR LOGIN [EMS_Doctor_Login] WITH DEFAULT_SCHEMA=[dbo]
END
GO

-- create nurse user for login
IF (SELECT COUNT(name) FROM sys.sysusers WHERE name = N'EMS_Nurse_User') < 1
BEGIN
	CREATE USER [EMS_Nurse_User] FOR LOGIN [EMS_Nurse_Login] WITH DEFAULT_SCHEMA=[dbo]
END
GO


-- add each user to respective roles
ALTER ROLE [EMS_Admin_Role] ADD MEMBER [EMS_Admin_User]
GO
ALTER ROLE [EMS_Doctor_Role] ADD MEMBER [EMS_Doctor_User]
GO
ALTER ROLE [EMS_Nurse_Role] ADD MEMBER [EMS_Nurse_User]
GO


