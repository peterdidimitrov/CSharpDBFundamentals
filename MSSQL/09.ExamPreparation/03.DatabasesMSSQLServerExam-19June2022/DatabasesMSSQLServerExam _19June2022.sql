--01. DDL

CREATE DATABASE Zoo
USE Zoo

CREATE TABLE Owners  (
		Id INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(50) NOT NULL,
		PhoneNumber VARCHAR(15) NOT NULL,
		[Address] VARCHAR(50)
)

CREATE TABLE AnimalTypes  (
		Id INT PRIMARY KEY IDENTITY,
		AnimalType VARCHAR(30) NOT NULL
)

CREATE TABLE Cages  (
		Id INT PRIMARY KEY IDENTITY,
		AnimalTypeId INT FOREIGN KEY REFERENCES AnimalTypes NOT NULL
)

CREATE TABLE Animals  (
		Id INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(30) NOT NULL,
		BirthDate Date NOT NULL,
		OwnerId INT FOREIGN KEY REFERENCES Owners,
		AnimalTypeId INT FOREIGN KEY REFERENCES AnimalTypes NOT NULL
)

CREATE TABLE AnimalsCages  (
		CageId INT FOREIGN KEY REFERENCES Cages NOT NULL,
		AnimalId INT FOREIGN KEY REFERENCES Animals NOT NULL
		PRIMARY KEY (CageId, AnimalId)
)

CREATE TABLE VolunteersDepartments  (
		Id INT PRIMARY KEY IDENTITY,
		DepartmentName VARCHAR(30) NOT NULL,
)

CREATE TABLE Volunteers  (
		Id INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(50) NOT NULL,
		PhoneNumber VARCHAR(15) NOT NULL,
		[Address] VARCHAR(50),
		AnimalId INT FOREIGN KEY REFERENCES Animals,
		DepartmentId INT FOREIGN KEY REFERENCES VolunteersDepartments NOT NULL
)

--02. Insert

INSERT INTO Volunteers([Name], PhoneNumber, [Address], AnimalId, DepartmentId) VALUES
		('Anita Kostova', '0896365412', 'Sofia, 5 Rosa str.', 15, 1),
		('Dimitur Stoev', '0877564223', NULL, 42, 4),
		('Kalina Evtimova', '0896321112', 'Silistra, 21 Breza str.', 9, 7),
		('Stoyan Tomov', '0898564100', 'Montana, 1 Bor str.', 18, 8),
		('Boryana Mileva', '0888112233', NULL, 31, 5)

INSERT INTO Animals([Name], BirthDate, OwnerId, AnimalTypeId) VALUES
		('Giraffe', '2018-09-21', 21, 1),
		('Harpy Eagle', '2015-04-17', 15, 3),
		('Hamadryas Baboon', '2017-11-02', NULL, 1),
		('Tuatara', '2021-06-30', 2, 4)

--03. Update

UPDATE Animals
   SET OwnerId = (SELECT [Id] FROM Owners WHERE [Name] = 'Kaloqn Stoqnov')
 WHERE OwnerId IS NULL

-- 04. Delete

UPDATE Volunteers
   SET AnimalId = NULL
 WHERE AnimalId = (SELECT Id FROM VolunteersDepartments
WHERE DepartmentName = 'Education program assistant')

DELETE FROM Volunteers
WHERE DepartmentId = (SELECT Id FROM VolunteersDepartments
WHERE DepartmentName = 'Education program assistant')

DELETE FROM VolunteersDepartments 
WHERE DepartmentName = 'Education program assistant'

--05. Volunteers

SELECT [Name],
	   PhoneNumber,
	   [Address],
	   AnimalId,
	   DepartmentId
FROM Volunteers 
ORDER BY [Name], AnimalId, DepartmentId


--06. Animals data


SELECT [Name],
	   [at].AnimalType,
	   FORMAT (BirthDate, 'dd.MM.yyyy')
FROM Animals AS a 
LEFT JOIN AnimalTypes AS [at] ON a.AnimalTypeId = [at].Id
ORDER BY [Name]

--07. Owners and Their Animals


SELECT TOP (5) [o].[Name] AS [Owner], 
	   COUNT(a.Id) AS [CountOfAnimals]
FROM Animals AS a 
INNER JOIN Owners AS [o] ON a.OwnerId = [o].Id
GROUP By [o].[Name]
ORDER BY [CountOfAnimals] DESC

--08. Owners, Animals and Cages

SELECT CONCAT([o].[Name], '-', a.[Name]) AS OwnersAnimals,
	   [o].PhoneNumber,
	   c.Id AS CageId
FROM Animals AS a 
INNER JOIN Owners AS [o] ON a.OwnerId = [o].Id
INNER JOIN AnimalsCages AS [ac] ON a.Id = [ac].AnimalId
INNER JOIN Cages AS c ON ac.CageId = c.Id
INNER JOIN AnimalTypes AS [at] ON a.AnimalTypeId = at.Id
WHERE AnimalType = 'mammals'
ORDER BY [o].[Name], a.[Name] DESC

--9.	Volunteers in Sofia

SELECT [Name],
	   PhoneNumber,
	   TRIM(REPLACE(REPLACE([Address], 'Sofia', ' '), ',', ' ')) AS [Address]
FROM Volunteers AS v
INNER JOIN VolunteersDepartments AS [vd] ON v.DepartmentId = [vd].Id
WHERE [Address] LIKE '%Sofia%' AND DepartmentName = 'Education program assistant' 
ORDER BY [v].[Name]

--10. Animals for Adoption

SELECT a.Name,
	   YEAR(a.BirthDate) AS BirthYear, 
	   at.AnimalType
FROM Animals AS a
INNER JOIN AnimalTypes AS [at] ON a.AnimalTypeId = [at].Id
WHERE DATEDIFF(year, a.BirthDate, '01/01/2022') < 5 AND AnimalType <> 'Birds' AND OwnerId IS NULL
ORDER BY [a].[Name]

GO
--11. All Volunteers in a Department

CREATE OR ALTER FUNCTION [udf_GetVolunteersCountFromADepartment] (@VolunteersDepartment VARCHAR(30))
		 RETURNS INT
                      AS 
                   BEGIN
						RETURN (SELECT COUNT([v].[Id])
											FROM Volunteers AS [v]
											JOIN VolunteersDepartments AS [vd] ON [v].DepartmentId = [vd].Id
											WHERE [vd].DepartmentName = @VolunteersDepartment)

				     END

GO

SELECT dbo.udf_GetVolunteersCountFromADepartment ('Education program assistant')

GO

--12. Animals with Owner or Not

CREATE PROC usp_AnimalsWithOwnersOrNot
		(@AnimalName VARCHAR(30))
AS
BEGIN
IF (SELECT OwnerId FROM Animals
			WHERE Name = @AnimalName) IS NULL
	BEGIN 
		SELECT Name, 'For adoption' AS OwnerName
			FROM Animals
			WHERE Name = @AnimalName
	END
	ELSE
	BEGIN
		SELECT a.Name, o.Name as OwnerName
			FROM Animals AS a
			JOIN Owners AS o ON o.Id = a.OwnerId
			WHERE a.Name = @AnimalName
	END
END

EXEC usp_AnimalsWithOwnersOrNot 'Pumpkinseed Sunfish'
EXEC usp_AnimalsWithOwnersOrNot 'Hippo'
EXEC usp_AnimalsWithOwnersOrNot 'Brown bear'
GO