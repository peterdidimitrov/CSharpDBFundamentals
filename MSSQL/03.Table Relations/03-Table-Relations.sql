--1.	One-To-One Relationship
--Create two tables and use appropriate data types.
--Insert the data from the example above. Alter the Persons table and make PersonID a primary key. Create a foreign key between Persons and Passports by using the PassportID column.



CREATE TABLE [Persons] (
			 [PersonID] INT NOT NULL
			,[FirstName] NVARCHAR(200) NOT NULL
			,[Salary] INT NOT NULL
			,[PassportID] INT NOT NULL
)

INSERT INTO [Persons]
VALUES
	(1, 'Roberto', 43300.00, 102)
	,(2, 'Tom', 56100.00, 103)
	,(3, 'Yana', 60200.00, 101)

CREATE TABLE [Passports] (
			 [PassportID] INT PRIMARY KEY IDENTITY(101,1)
	        ,[PassportNumber] VARCHAR(8) NOT NULL
)

INSERT INTO [Passports]
VALUES
	('N34FG21B')
	,('K65LO4R7')
	,('ZE657QP2')

	ALTER TABLE [Persons]
ADD PRIMARY KEY([PersonID])



	ALTER TABLE [Persons]
ADD FOREIGN KEY ([PassportID]) 
	 REFERENCES [Passports]([PassportID]);


--2.	One-To-Many Relationship
--Create two tables and use appropriate data types.
--Insert the data from the example above and add primary keys and foreign keys.

CREATE TABLE [Models] (
			 [ModelID] INT PRIMARY KEY IDENTITY(101, 1)
			,[Name] NVARCHAR(200) NOT NULL
			,[ManufacturerID] INT NOT NULL
)

INSERT INTO [Models]
	 VALUES
			('X1', 1)
			,('i6', 1)
			,('Model S', 2)
			,('Model  X', 2)
			,('Model  3', 2)
			,('Nova', 3)

CREATE TABLE [Manufacturers] (
			 [ManufacturerID] INT PRIMARY KEY IDENTITY(1,1)
	         ,[PassportNumber] VARCHAR(8) NOT NULL
			 ,[EstablishedOn] DATETIME2
)

INSERT INTO [Manufacturers]
	 VALUES
			('BMW', '07/03/1916')
			,('Tesla', '01/01/2003')
			,('Lada', '01/05/1966')


	ALTER TABLE [Models]
ADD FOREIGN KEY ([ManufacturerID]) 
	 REFERENCES [Manufacturers]([ManufacturerID]);

--3.	Many-To-Many Relationship
--Create three tables and use appropriate data types.
--Insert the data from the example above and add primary keys and foreign keys. Keep in mind that the table "StudentsExams" should have a composite primary key.

CREATE TABLE [Students] (
			 [StudentID] INT PRIMARY KEY IDENTITY(1, 1) 
			 ,[Name] NVARCHAR(200) NOT NULL,
)

INSERT INTO [Students]
	 VALUES
			('Mila')
			,('Toni')
			,('Ron')

CREATE TABLE [Exams] (
			 [ExamID] INT PRIMARY KEY IDENTITY(101,1)
			 ,[Name] VARCHAR(100) NOT NULL,
)

INSERT INTO [Exams]
	 VALUES
			('SpringMVC')
			,('Neo4j')
			,('Oracle 11g')

CREATE TABLE [StudentsExams] (
			 [StudentAndExamID] INT PRIMARY KEY ([StudentID], [ExamID]) IDENTITY(1,1)
			 ,[StudentID] INT FOREIGN KEY REFERENCES [Students]([StudentID])
			 ,[ExamID] INT FOREIGN KEY REFERENCES [Exams](ExamID)
)


INSERT INTO [StudentsExams]
	 VALUES
			(1, 101)
			,(1, 102)
			,(2, 101)
			,(3, 103)
			,(2, 102)
			,(2, 103)

--	4.	Self-Referencing 
--Create one table and use appropriate data types.
--Insert the data from the example above and add primary keys and foreign keys. The foreign key should be between ManagerId and TeacherId.

CREATE TABLE [Teachers] (
			 [TeacherID] INT PRIMARY KEY IDENTITY (101, 1)
			 ,[Name] NVARCHAR(100)  NOT NULL
			 ,[ManagerID] INT FOREIGN KEY REFERENCES [Teachers]([TeacherID])
)

INSERT INTO [Teachers]
	 VALUES
			('John', NULL)
			,('Maya', 106)
			,('Silvia', 106)
			,('Ted', 105)
			,('Mark', 101)
			,('Greta', 101)

--5.	Online Store Database
--Create a new database and design the following structure:

CREATE DATABASE [Store]
			USE [Store]
   CREATE TABLE [Cities] (
				[CityID] INT PRIMARY KEY IDENTITY (1, 1)
				,[Name] NVARCHAR(100)
)

   CREATE TABLE [Customers] (
				[CustomerID] INT PRIMARY KEY IDENTITY (1, 1)
				,[Name] NVARCHAR(100)
				,[Birthday] DATETIME2
				,[CityID] INT FOREIGN KEY REFERENCES [Cities]([CityID])
)

   CREATE TABLE [Orders] (
				[OrderID] INT PRIMARY KEY IDENTITY (1, 1)
				,[CustomerID] INT FOREIGN KEY REFERENCES [Customers]([CustomerID])
)

   CREATE TABLE [ItemTypes] (
				[ItemTypeID] INT PRIMARY KEY IDENTITY (1, 1)
				,[Name] NVARCHAR(100)
)

   CREATE TABLE [Items] (
				[ItemID] INT PRIMARY KEY IDENTITY (1, 1)
				,[Name] NVARCHAR(100)
				,[ItemTypeID] INT FOREIGN KEY REFERENCES [ItemTypes]([ItemTypeID])
)

   CREATE TABLE [OrderItems] (
				[OrdereAndItemID] INT PRIMARY KEY ([OrderID], [ItemID]) IDENTITY(1,1)
				,[OrderID] INT FOREIGN KEY REFERENCES [Orders]([OrderID])
				,[ItemID] INT FOREIGN KEY REFERENCES [Items]([ItemID])
)



--6.	University Database
--Create a new database and design the following structure:


CREATE DATABASE [University]
			 GO
			USE [University]

   CREATE TABLE [Majors] (
				[MajorID] INT PRIMARY KEY IDENTITY(1,1)
				,[Name] NVARCHAR(100)
)

   CREATE TABLE [Students] (
				[StudentID] INT PRIMARY KEY IDENTITY (1, 1)
				,[StudentNumber] VARCHAR(20)
				,[StudentName] NVARCHAR(100)
				,[MajorID] INT FOREIGN KEY REFERENCES [Majors]([MajorID])
)

   CREATE TABLE [Payments] (
				[PaymentID] INT PRIMARY KEY IDENTITY (1, 1)
				,[PaymentDate] DATETIME2
				,[PaymentAmount] MONEY
				,[StudentID] INT FOREIGN KEY REFERENCES [Students]([StudentID])
)

   CREATE TABLE [Subjects] (
				[SubjectID] INT PRIMARY KEY IDENTITY(1,1)
				,[SubjectName] NVARCHAR(100)
)

   CREATE TABLE [Agenda] (
				[StudentAndSubjectID] INT PRIMARY KEY ([StudentID], [SubjectID]) IDENTITY(1,1)
				,[StudentID] INT FOREIGN KEY REFERENCES [Students]([StudentID])
				,[SubjectID] INT FOREIGN KEY REFERENCES [Subjects]([SubjectID])
)

--9.	*Peaks in Rila
--Display all peaks for "Rila" mountain. Include:
--•	MountainRange
--•	PeakName
--•	Elevation
--Peaks should be sorted by elevation descending.

USE [Geography]
SELECT * FROM [Mountains]
WHERE [MountainRange] = 'Rila';

SELECT [Mountains].[MountainRange],  [Peaks].[PeakName], [Peaks].[Elevation] FROM [Peaks] 
INNER JOIN [Mountains] ON [Mountains].[Id]=[Peaks].[MountainId]
WHERE [MountainID] = '17'
ORDER BY [Elevation] DESC

