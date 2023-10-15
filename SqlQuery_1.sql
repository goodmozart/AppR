CREATE TABLE UsersInformation (
    Id INT IDENTITY(1,1) PRIMARY KEY,
	DateAdded datetime NULL,
	UserId NVARCHAR(20) NULL,
	HashPassword NVARCHAR(max) NULL,
	Salt NVARCHAR(max) NULL,
	FirstName NVARCHAR(100) NULL,
	MiddleName NVARCHAR(100) NULL,
	LastName NVARCHAR(100) NULL,
	Gender NVARCHAR(20) NULL,
	Email NVARCHAR(50) NULL,
	MobileNumber NVARCHAR(30) NULL,
	GroupDept NVARCHAR(50) NULL,
	UserRole NVARCHAR(50) NULL,
	UserStatus char(1) NULL,
	loginAttempt char(1) NULL,
);

drop table UsersInformation

  insert into UsersInformation (DateAdded, UserId, HashPassword, Salt, EmployeeName, Email, MobileNumber, GroupDept, UserRole, UserStatus, loginAttempt) 
  values ('09-24-2023','admin','ALaz0FGJ9gw22xLaA7J+93GyArXfTAC30sGAxcr06/rm9huQ5FuhLzr0aUMmeHOA6A==','rOTy5ENzWGiU6cDsut3NAw==','Admin Admin','admin@gmail.com','092121212','admingroup','admin','2','0')

 CREATE TABLE EmailTemplate (
    Id INT IDENTITY(1,1) PRIMARY KEY,
	EmailType NVARCHAR(200) NULL,
	Subject NVARCHAR(max) NULL,
	Content NVARCHAR(max) NULL,
);

Insert into EmailTemplate (EmailType, Subject, Content)
values('Credit Notification','Subject Credit Notification','Credit Notification This is an auto-generated email. Please do not reply to this email.')
Insert into EmailTemplate (EmailType, Subject, Content)
values('Deposit Notification','Subject Deposit Notification','Deposit Notification This is an auto-generated email. Please do not reply to this email.')
Insert into EmailTemplate (EmailType, Subject, Content)
values('OTP Notification','OTP Credit Notification','OTP Notification This is an auto-generated email. Please do not reply to this email.')