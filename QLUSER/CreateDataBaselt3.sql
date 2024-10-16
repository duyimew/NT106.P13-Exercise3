Create database mydatabase;
go
use mydatabase;
go
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(256) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
	FullName  NVARCHAR(100) NOT NULL,
	Birthday date NOT NULL,
);
