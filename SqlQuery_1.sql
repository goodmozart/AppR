CREATE TABLE UsersInformation (
    Id INT IDENTITY(1,1) PRIMARY KEY,
	DateAdded datetime NULL,
	UserId NVARCHAR(20) NULL,
	HashPassword NVARCHAR(max) NULL,
	Salt NVARCHAR(max) NULL,
	EmployeeName NVARCHAR(100) NULL,
	Email NVARCHAR(50) NULL,
	MobileNumber NVARCHAR(30) NULL,
	GroupDept NVARCHAR(50) NULL,
	UserRole NVARCHAR(50) NULL,
	UserStatus NVARCHAR(50) NULL,
);

drop table UsersInformation

  insert into UsersInformation (DateAdded, UserId, HashPassword, Salt, EmployeeName, Email, MobileNumber, GroupDept, UserRole, UserStatus) 
  values ('09-24-2023','admin','ALaz0FGJ9gw22xLaA7J+93GyArXfTAC30sGAxcr06/rm9huQ5FuhLzr0aUMmeHOA6A==','rOTy5ENzWGiU6cDsut3NAw==','Admin Admin','admin@gmail.com','092121212','admingroup','admin','2')
