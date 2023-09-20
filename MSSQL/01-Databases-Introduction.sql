CREATE DATABASE [Minions]

USE [Minions]
CREATE TABLE [Minions](
	[Id] INT PRIMARY KEY, 
	[Name] VARCHAR(100),
	[Age] INT
)

CREATE TABLE [People] (
	[Id] INT PRIMARY KEY, 
	[Name] NVARCHAR(200) NOT NULL,
	[Picture] VARCHAR(MAX),
	[Height] FLOAT,
	[Weight] FLOAT,
	[Gender] CHAR NOT NULL,
	[Birthdate] VARCHAR(10) NOT NULL,
	[Biography] NVARCHAR(MAX)
)

ALTER TABLE [Minions]
		ADD  [TownId] INT FOREIGN KEY REFERENCES [Towns] ([Id])

INSERT INTO [People]
VALUES
	(1, 'Sofia', 'efewf', 3.8, 4.7, 'm', '10.08.1987', 'ewfwfwe'),
	(2, 'Sofia', 'efewf', 3.8, 4.7, 'm', '10.08.1987', 'ewfwfwe'),
	(3, 'Sofia', 'efewf', 3.8, 4.7, 'm', '10.08.1987', 'ewfwfwe'),
	(4, 'Sofia', 'efewf', 3.8, 4.7, 'm', '10.08.1987', 'ewfwfwe'),
	(5, 'Sofia', 'efewf', 3.8, 4.7, 'm', '10.08.1987', 'ewfwfwe');

INSERT INTO [Minions]
VALUES
	(1, 'Kevin', 22, 1),
	(2, 'Bob', 15, 3),
	(3, 'Steward', NULL, 2)

TRUNCATE TABLE [Minions]

	--DROP TABLE Users
	USE [Minions]
CREATE TABLE [Users] (
	[Id] BIGINT PRIMARY KEY IDENTITY,
	[Username]  VARCHAR(30) NOT NULL,
	[Password] VARCHAR(26) NOT NULL,
	[ProfilePicture] VARBINARY(MAX) CHECK(LEN([ProfilePicture]) >= 900000),
	[LastLoginTime] DATETIME2,
	[IsDeleted] BIT
	)
INSERT INTO [Users]
VALUES
	('Peter', '1264567', null, '12-01-16 12:32', 0),
	('Peter2', '1234567', null, '12-01-16 12:32', 1),
	('Peter3', '2234567', null, '12-01-16 12:32', 0),
	('Peter4', '3234567', null, '12-01-16 12:32', 1),
	('Peter5', '4234567', null, '12-01-16 12:32', 0);

ALTER TABLE [Users] DROP CONSTRAINT PK__Users__3214EC07DAC1C436;
ALTER TABLE [Users] ADD CONSTRAINT PK__IdUsername PRIMARY KEY ([Id], [Username]);

ALTER TABLE [Users] DROP CONSTRAINT PK__Users__3214EC07DAC1C436;

ALTER TABLE [Users] ADD CONSTRAINT CHK_PasswordMinLen CHECK(LEN([Password]) >= 5);

ALTER TABLE [Users] ADD CONSTRAINT DF_LastLoginTime DEFAULT GETDATE() FOR [LastLoginTime];

INSERT INTO Users ([Username],[Password],[IsDeleted])
VALUES
	('Ivan', '1264567', 0);
SELECT * FROM [Users]

ALTER TABLE [Users] DROP CONSTRAINT PK__IdUsername;

ALTER TABLE [Users] ADD CONSTRAINT PK_Id PRIMARY KEY ([Id]);

ALTER TABLE [Users] ADD CONSTRAINT UC_Username UNIQUE ([Username]);

ALTER TABLE [Users] ADD CONSTRAINT CHK_UsernameLen CHECK(LEN([Username]) >= 3)

--13 

--Using SQL queries create Movies database with the following entities:
--•	Directors (Id, DirectorName, Notes)
--•	Genres (Id, GenreName, Notes)
--•	Categories (Id, CategoryName, Notes)
--•	Movies (Id, Title, DirectorId, CopyrightYear, Length, GenreId, CategoryId, Rating, Notes)
--Set the most appropriate data types for each column. Set a primary key to each table. Populate each table with exactly 5 records. Make sure the columns that are present in 2 tables would be of the same data type. Consider which fields are always required and which are optional. Submit your CREATE TABLE and INSERT statements as Run queries & check DB.


CREATE DATABASE [Movies]

USE [Movies]

CREATE TABLE [Directors] (
	[Id] INT PRIMARY KEY,
	[DirectorName] NVARCHAR(100) NOT NULL,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Directors]
VALUES
	(1, 'Kevin', 22),
	(2, 'Bob', 15),
	(3, 'Steward', 2),
	(4, 'Sam', 2),
	(5, 'Stephan', 2);

CREATE TABLE [Genres] (
	[Id] INT PRIMARY KEY,
	[GenreName] NVARCHAR(100) NOT NULL,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Genres]
VALUES
	(1, 'Horor', 22),
	(2, 'Comedy', 15),
	(3, 'Drama', 2),
	(4, 'Drama', 2),
	(5, 'Drama', 2);



CREATE TABLE [Categories] (
	[Id] INT PRIMARY KEY,
	[CategoryName] NVARCHAR(100) NOT NULL,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Categories]
VALUES
	(1, 'Hi', 22),
	(2, 'Low', 15),
	(3, 'Middle', 2),
	(4, 'Low', 2),
	(5, 'Hi', 2);

CREATE TABLE [Movies] (
	[Id] INT PRIMARY KEY,
	[Title] NVARCHAR(100),
	[DirectorId] BIGINT NOT NULL,
	[CopyrightYear] NVARCHAR(100),
	[Length] INT,
	[GenreId] BIGINT NOT NULL,
	[CategoryId] BIGINT NOT NULL,
	[Rating] TINYINT,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Movies]
VALUES
	(1, 'Hi', 1, 1922, 23, 5, 1, 10, 'WOW'),
	(2, 'Low', 2, 2015, 23, 4, 2, 10, 'WOW'),
	(3, 'Middle', 3, 2022, 23, 3, 3, 10, 'WOW'),
	(4, 'Low', 4, 2023, 23, 2, 4, 10, 'WOW'),
	(5, 'Hi', 5, 2000, 23, 1, 5, 10, 'WOW');


--	14.	Car Rental Database

--Using SQL queries create CarRental database with the following entities:

CREATE DATABASE [CarRental]

--•	Categories (Id, CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate)

USE [CarRental]

CREATE TABLE [Categories] (
	[Id] INT PRIMARY KEY,
	[CategoryName] NVARCHAR(100) NOT NULL,
	[DailyRate] TINYINT,
	[WeeklyRate] TINYINT,
	[MonthlyRate] TINYINT,
	[WeekendRate] TINYINT,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Categories]
VALUES
	(1, 'car', 22, 4, 7, 7, 'GOOD'),
	(2, 'truck', 15, 4, 7, 7, 'GOOD'),
	(3, 'motor', 2, 4, 7, 7, 'GOOD')


--•	Cars (Id, PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Available)

CREATE TABLE [Cars] (
	[Id] INT PRIMARY KEY,
	[PlateNumber] VARCHAR(10) NOT NULL,
	[Manufacturer] NVARCHAR(100) NOT NULL,
	[Model] NVARCHAR(100) NOT NULL,
	[CarYear] INT,
	[CategoryId] INT,
	[Doors] TINYINT,
	[Picture] VARBINARY(MAX),
	[Condition] NVARCHAR(100),
	[Available] NVARCHAR(100)
)

INSERT INTO [Cars]
VALUES
	(1, 'CA1050HT', 'Audi', 'A8', 1999, 1, 4, 010101, NULL, NULL),
	(2, 'CB1250XP', 'IVECO', 'MONSTER', 2010, 2, 2, 010101, NULL, NULL),
	(3, 'CB5500BP', 'HONDA', 'Bumbulbi', 2023, 1, NULL, 010101, NULL, NULL)


--•	Employees (Id, FirstName, LastName, Title, Notes)

CREATE TABLE [Employees] (
	[Id] INT PRIMARY KEY,
	[FirstName] VARCHAR(100) NOT NULL,
	[LastName] NVARCHAR(100) NOT NULL,
	[Title] NVARCHAR(100) NOT NULL,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Employees]
VALUES
	(1, 'Peter', 'Dimitrov', 'Mr', NULL),
	(2, 'Ema', 'Dimitrov', 'Ms', NULL),
	(3, 'Dimitar', 'Dimitrov', 'Mr', NULL)

--•	Customers (Id, DriverLicenceNumber, FullName, Address, City, ZIPCode, Notes)

CREATE TABLE [Customers] (
	[Id] INT PRIMARY KEY,
	[DriverLicenceNumber] INT NOT NULL,
	[FullName] NVARCHAR(100) NOT NULL,
	[Address] NVARCHAR(100),
	[City] NVARCHAR(100),
	[ZIPCode] INT,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Customers]
VALUES
	(1, 8545412, 'Petar Dimitrov', 'str. Mkedonia 55', 'Sofia', 1606, NULL),
	(2, 8645412, 'Ema Dimitrov', 'str. Mkedonia 55', 'Sofia', 1606, NULL),
	(3, 8575412, 'Dimitar Dimitrov', 'str. Mkedonia 55', 'Sofia', 1606, NULL)

--•	RentalOrders (Id, EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes)

CREATE TABLE [RentalOrders] (
	[Id] INT PRIMARY KEY,
	[EmployeeId] INT NOT NULL,
	[CustomerId] INT NOT NULL,
	[CarId] INT,
	[TankLevel] INT,
	[KilometrageStart] INT,
	[KilometrageEnd] INT,
	[TotalKilometrage] INT,
	[StartDate] DATETIME2,
	[EndDate] DATETIME2,
	[TotalDays] INT,
	[RateApplied] INT, 
	[TaxRate] INT, 
	[OrderStatus] NVARCHAR(100), 
	[Notes] NVARCHAR(100)
)

INSERT INTO [RentalOrders]
VALUES
	(1, 2, 1, 4846454, 60, 12000, 15000, 3000, '07.08.2023', '07.09.2023', 31, 10, NULL, NULL, NULL),
	(2, 3, 2, 4945454, 60,12000, 15000, 3000, '07.08.2023', '07.09.2023', 31, 10, NULL, NULL, NULL),
	(3, 1, 3, 4845474, 60, 12000, 15000, 3000, '07.08.2023', '07.09.2023', 31, 10, NULL, NULL, NULL)


--15.	Hotel Database
--Using SQL queries create Hotel database with the following entities:

CREATE DATABASE [Hotel]

USE [Hotel]

--•	Employees (Id, FirstName, LastName, Title, Notes)

CREATE TABLE [Employees](
	[Id] INT PRIMARY KEY, 
	[FirstName] NVARCHAR(100) NOT NULL, 
	[LastName] NVARCHAR(100) NOT NULL, 
	[Title] NVARCHAR(100), 
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Employees]
VALUES
	(1, 'Peter', 'Dimitrov', 'Mr', NULL),
	(2, 'Donka', 'Ivanova', 'Ms', NULL),
	(3, N'Минчо', N'Празников', 'Mr', NULL)

--•	Customers (AccountNumber, FirstName, LastName, PhoneNumber, EmergencyName, EmergencyNumber, Notes)

CREATE TABLE [Customers](
	[AccountNumber] INT PRIMARY KEY, 
	[FirstName] NVARCHAR(100) NOT NULL, 
	[LastName] NVARCHAR(100) NOT NULL,  
	[PhoneNumber] INT NOT NULL, 
	[EmergencyName] NVARCHAR(100) NOT NULL, 
	[EmergencyNumber] INT NOT NULL,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Customers]
VALUES
	(1245, 'Dimitar', 'Dimitrov', 15445658, 'Petar Dimitrov', 4185168, NULL),
	(1235, 'Ivan', 'Dimitrov', 15445658, N'Григор Дочев', 4185168, NULL),
	(1255, N'Пенка', N'Мишева', 15445658, N'Първан Мишев', 4185168, NULL)


--•	RoomStatus (RoomStatus, Notes)

CREATE TABLE [RoomStatus](
	[RoomStatus] VARCHAR(100) PRIMARY KEY, 
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [RoomStatus]
VALUES
	('Reserved', NULL),
	('Free', NULL),
	('InPrepare', NULL)

--•	RoomTypes (RoomType, Notes)

CREATE TABLE [RoomTypes](
	[RoomType] VARCHAR(100) PRIMARY KEY, 
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [RoomTypes]
VALUES
	('Appartment', NULL),
	('TwoBeds', NULL),
	('OneBed', NULL)


--•	BedTypes (BedType, Notes)

CREATE TABLE [BedTypes](
	[BedType] VARCHAR(100) PRIMARY KEY, 
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [BedTypes]
VALUES
	('OnePerson', NULL),
	('TwoPercons', NULL),
	('ThreePersons', NULL)

--•	Rooms (RoomNumber, RoomType, BedType, Rate, RoomStatus, Notes)

CREATE TABLE [Rooms](
	[RoomNumber] INT PRIMARY KEY, 
	[RoomType] VARCHAR(100), 
	[BedType] VARCHAR(100), 
	[Rate] INT, 
	[RoomStatus] VARCHAR(100),
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Rooms]
VALUES
	(1,'Appartment', 'ThreePersons', NULL, NULL, NULL),
	(2,'TwoBeds', 'TwoPercons', NULL, NULL, NULL),
	(3, 'TwoBeds', 'OnePerson', NULL, NULL, NULL)

--•	Payments (Id, EmployeeId, PaymentDate, AccountNumber, FirstDateOccupied, LastDateOccupied, TotalDays, AmountCharged, TaxRate, TaxAmount, PaymentTotal, Notes)

CREATE TABLE [Payments](
	[Id] INT PRIMARY KEY, 
	[EmployeeId] INT NOT NULL, 
	[PaymentDate] DATETIME2 NOT NULL, 
	[AccountNumber] INT NOT NULL, 
	[FirstDateOccupied] DATETIME2 NOT NULL,
	[LastDateOccupied] DATETIME2 NOT NULL,
	[TotalDays] INT NOT NULL,
	[AmountCharged] DECIMAL NOT NULL, 
	[TaxRate] DECIMAL NOT NULL, 
	[TaxAmount] DECIMAL NOT NULL, 
	[PaymentTotal] DECIMAL NOT NULL,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Payments]
VALUES
	(1, 3, '07.08.2023', 1245, '07.08.2023', '07.08.2023', 30, 10000, 100, 100, 10200, NULL),
	(2, 1, '07.08.2023', 1255, '07.08.2023', '07.08.2023', 30, 10000, 100, 100, 10200, NULL),
	(3, 2, '07.08.2023', 1235, '07.08.2023', '07.08.2023', 30, 10000, 100, 100, 10200, NULL)

--•	Occupancies (Id, EmployeeId, DateOccupied, AccountNumber, RoomNumber, RateApplied, PhoneCharge, Notes)


CREATE TABLE [Occupancies](
	[Id] INT PRIMARY KEY, 
	[EmployeeId] INT NOT NULL, 
	[DateOccupied] DATETIME2 NOT NULL, 
	[AccountNumber] INT NOT NULL, 
	[RoomNumber] INT NOT NULL, 
	[RateApplied] INT, 
	[PhoneCharge] DECIMAL,
	[Notes] NVARCHAR(MAX)
)

INSERT INTO [Occupancies]
VALUES
	(1, 3, '07.08.2023', 1245, 3, 10, 12, NULL),
	(2, 1, '07.08.2023', 1255, 1, 10, 12, NULL),
	(3, 2, '07.08.2023', 1235, 2, 10, 12, NULL)


--16.	Create SoftUni Database
--Now create bigger database called SoftUni. You will use the same database in the future tasks. It should hold information about

CREATE DATABASE [SoftUni]
USE [SoftUni]

--•	Towns (Id, Name)

CREATE TABLE [Towns](
	[Id] INT PRIMARY KEY IDENTITY(1,1), 
	[Name] NVARCHAR(100) NOT NULL
)

--•	Addresses (Id, AddressText, TownId)

CREATE TABLE [Addresses](
	[Id] INT PRIMARY KEY IDENTITY(1,1), 
	[AddressText] NVARCHAR(100) NOT NULL,
	[TownId] INT FOREIGN KEY REFERENCES [Towns](Id) NOT NULL
)

--•	Departments (Id, Name)

CREATE TABLE [Departments](
	[Id] INT PRIMARY KEY IDENTITY(1,1), 
	[Name] NVARCHAR(100) NOT NULL
)

--•	Employees (Id, FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId)

CREATE TABLE [Employees](
	[Id] INT PRIMARY KEY IDENTITY(1,1), 
	[FirstName] NVARCHAR(100) NOT NULL, 
	[MiddleName] NVARCHAR(100), 
	[LastName] NVARCHAR(100) NOT NULL, 
	[JobTitle] NVARCHAR(100) NOT NULL, 
	[DepartmentId] INT FOREIGN KEY REFERENCES [Departments]([Id]) NOT NULL, 
	[HireDate] DATETIME2, 
	[Salary] INT, 
	[AddressId] INT FOREIGN KEY REFERENCES [Addresses]([Id])
)
drop table [Employees]

--The Id columns are auto incremented, starting from 1 and increased by 1 (1, 2, 3, 4…). Make sure you use appropriate data types for each column. Add primary and foreign keys as constraints for each table. Use only SQL queries. Consider which fields are always required and which are optional.

	

--use master
--Drop DATABASE SoftUni

--18.	Basic Insert
--Use the SoftUni database and insert some data using SQL queries.

--•	Towns: Sofia, Plovdiv, Varna, Burgas

INSERT INTO [Towns]
VALUES
	('Sofia'),
	('Plovdiv'),
	('Varna'),
	('Burgas')

--•	Departments: Engineering, Sales, Marketing, Software Development, Quality Assurance

INSERT INTO [Departments]
VALUES
	('Engineering'),
	('Sales'),
	('Marketing'),
	('Software Development'),
	('Quality Assurance')

--•	Employees:
--Name					Job Title		Department				Hire Date	Salary
--Ivan Ivanov Ivanov	.NET Developer	Software Development	01/02/2013	3500.00
--Petar Petrov Petrov	Senior Engineer	Engineering				02/03/2004	4000.00
--Maria Petrova Ivanova	Intern			Quality Assurance		28/08/2016	525.25
--Georgi Teziev Ivanov	CEO				Sales					09/12/2007	3000.00
--Peter Pan Pan			Intern			Marketing				28/08/2016	599.88

INSERT INTO [Employees]
VALUES
	('Ivan', 'Ivanov', 'Ivanov',	'.NET Developer',	4, '02-01-2013', 3500.00, NULL),
	('Petar', 'Petrov', 'Petrov',	'Senior Engineer',	1,	'03-02-2004', 4000.00, NULL),
	('Maria', 'Petrova', 'Ivanova', 'Intern',			5,	'08-28-2016', 525.25, NULL),
	('Georgi', 'Teziev', 'Ivanov',	'CEO',				2,	'12-09-2007', 3000.00, NULL),
	('Petar',	'Pan',	  'Pan',	'Intern',			3,	'08-28-2016', 599.88, NULL)

--	19.	Basic Select All Fields
--Use the SoftUni database and first select all records from the Towns, then from Departments and finally from Employees table. Use SQL queries and submit them to Judge at once. Submit your query statements as Prepare DB & Run queries.


SELECT * FROM [Towns]
SELECT * FROM [Departments]
SELECT * FROM [Employees]


--20.	Basic Select All Fields and Order Them
--Modify the queries from the previous problem by sorting:
--•	Towns - alphabetically by name
--•	Departments - alphabetically by name
--•	Employees - descending by salary
--Submit your query statements as Prepare DB & Run queries.

SELECT   * 
FROM     [Towns] 
ORDER BY [Name] 
ASC
SELECT   * 
FROM	 [Departments] 
ORDER BY [Name] 
ASC
SELECT	 * 
FROM	 [Employees] 
ORDER BY [Salary] 
DESC

--21.	Basic Select Some Fields
--Modify the queries from the previous problem to show only some of the columns. For table:
--•	Towns – Name
--•	Departments – Name
--•	Employees – FirstName, LastName, JobTitle, Salary
--Keep the ordering from the previous problem. Submit your query statements as Prepare DB & Run queries.

SELECT   [Name]
FROM     [Towns]
ORDER BY [Name] 
ASC
SELECT   [Name] 
FROM	 [Departments] 
ORDER BY [Name] 
ASC
SELECT	 [FirstName]
		,[LastName]
		,[JobTitle]
		,[Salary] 
FROM	 [Employees] 
ORDER BY [Salary] 
DESC

--22.	Increase Employees Salary
--Use SoftUni database and increase the salary of all employees by 10%. Then show only Salary column for all the records in the Employees table. Submit your query statements as Prepare DB & Run queries.

UPDATE [Employees]
   SET [Salary] = [Salary] * 1.1
SELECT [Salary] 
  FROM [Employees]

--23.	Decrease Tax Rate
--Use Hotel database and decrease tax rate by 3% to all payments. Then select only TaxRate column from the Payments table. Submit your query statements as Prepare DB & Run queries.

   USE [Hotel]
UPDATE [Payments]
   SET [TaxRate] = [TaxRate] * 0.97
SELECT [TaxRate] FROM [Payments]


--24.	Delete All Records
--Use Hotel database and delete all records from the Occupancies table. Use SQL query. Submit your query statements as Run skeleton, run queries & check DB.


           USE [Hotel]
TRUNCATE TABLE [Occupancies]
		SELECT * 
		  FROM [Payments]