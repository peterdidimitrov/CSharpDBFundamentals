--01. DDL
CREATE DATABASE [Airport]
USE [Airport]

CREATE TABLE Passengers (
		[Id] INT PRIMARY KEY IDENTITY,
		FullName VARCHAR(100) UNIQUE NOT NULL,
		Email VARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Pilots (
		[Id] INT PRIMARY KEY IDENTITY,
		FirstName VARCHAR(30) UNIQUE NOT NULL,
		LastName VARCHAR(30) UNIQUE NOT NULL,
		Age TINYINT NOT NULL CHECK (Age BETWEEN 21 AND 62),
		Rating FLOAT CHECK (Rating BETWEEN 0.0 AND 10.0)
)

CREATE TABLE AircraftTypes (
		[Id] INT PRIMARY KEY IDENTITY,
		TypeName VARCHAR(30) UNIQUE NOT NULL
)

CREATE TABLE Aircraft (
		[Id] INT PRIMARY KEY IDENTITY,
		Manufacturer VARCHAR(25) NOT NULL,
		Model VARCHAR(30) NOT NULL,
		[Year] INT NOT NULL,
		FlightHours INT,
		Condition CHAR  NOT NULL, 
		TypeId INT FOREIGN KEY REFERENCES AircraftTypes([Id]) NOT NULL
)

CREATE TABLE PilotsAircraft (
		AircraftId INT FOREIGN KEY REFERENCES Aircraft([Id]) NOT NULL, 
		PilotId INT FOREIGN KEY REFERENCES Pilots([Id]) NOT NULL
		PRIMARY KEY (AircraftId, PilotId)
)

CREATE TABLE Airports (
		[Id] INT PRIMARY KEY IDENTITY, 
		AirportName VARCHAR(70) UNIQUE NOT NULL,
		Country VARCHAR(100) UNIQUE NOT NULL
)

CREATE TABLE FlightDestinations (
		[Id] INT PRIMARY KEY IDENTITY,
		AirportId INT FOREIGN KEY REFERENCES Airports(Id) NOT NULL,  
		[Start] DATETIME NOT NULL,
		AircraftId INT FOREIGN KEY REFERENCES Aircraft(Id) NOT NULL,
		PassengerId INT FOREIGN KEY REFERENCES Passengers(Id) NOT NULL,
		TicketPrice DECIMAL(18,2) DEFAULT 15 NOT NULL
)

--02. Insert

DECLARE @count INT = 0

WHILE @count <= 10
BEGIN
INSERT INTO Passengers (FullName, Email) VALUES
		(CONCAT_WS(' ', 
		(
			SELECT FirstName 
			FROM Pilots 
			WHERE Id = 5 + @count
		), 
		(
			SELECT LastName 
			FROM Pilots
			WHERE Id = 5 + @count
		)),  CONCAT(
		(
			SELECT FirstName 
			FROM Pilots
			WHERE Id = 5 + @count
		), 
		(
			SELECT LastName 
			FROM Pilots
			WHERE Id = 5 + @count
		), '@gmail.com'))


		SET @count += 1;
END
--03. Update

UPDATE Aircraft
   SET Condition = 'A'
 WHERE (Condition = 'C' OR Condition ='B') AND (FlightHours IS NULL OR FlightHours <= 100) AND [Year] >= 2013

-- 04. Delete


DELETE FlightDestinations
WHERE Id IN (SELECT Id FROM Passengers
WHERE LEN(FullName) <= 10)


DELETE Passengers
WHERE LEN(FullName) <= 10


--05. Aircraft

  SELECT Manufacturer,
		 Model,
		 FlightHours,
		 Condition
    FROM Aircraft
ORDER BY FlightHours DESC

--06. Pilots and Aircraft

   SELECT p.FirstName,
		  p.LastName,
		  a.Manufacturer,
		  a.Model,
		  a.FlightHours
     FROM Pilots p
LEFT JOIN PilotsAircraft pa ON p.Id = pa.PilotId
LEFT JOIN Aircraft a ON pa.AircraftId = a.Id
WHERE a.Model IS NOT NULL AND FlightHours < 304
 ORDER BY FlightHours DESC, FirstName 

 --07. Top 20 Flight Destinations

  SELECT TOP 20 fd.Id DestinationId,
            fd.Start,
			p.FullName,
			a.AirportName,
			fd.TicketPrice
     FROM FlightDestinations fd
LEFT JOIN Airports a ON fd.AirportId = a.Id
LEFT JOIN Passengers p ON fd.PassengerId = p.Id
WHERE DAY(fd.[Start]) % 2 = 0
 ORDER BY TicketPrice DESC, a.AirportName 

 --08. Number of Flights for Each Aircraft

  SELECT a.Id AircraftId,
		a.Manufacturer,
		a.FlightHours,
		COUNT(fd.Id) FlightDestinationsCount, 
		ROUND(AVG(fd.TicketPrice), 2) AvgPrice
     FROM Aircraft a
LEFT JOIN FlightDestinations fd ON a.Id = fd.AircraftId
LEFT JOIN Passengers p ON fd.PassengerId = p.Id
 GROUP BY a.Id,  a.Manufacturer, a.FlightHours
 HAVING COUNT(fd.AircraftId) >= 2
 ORDER BY COUNT(fd.Id) DESC, AircraftId 

 --09. Regular Passengers
 
  SELECT p.FullName,
         COUNT(AircraftId) CountOfAircraft, 
		 SUM(fd.TicketPrice) TotalPayed
     FROM Passengers p
LEFT JOIN FlightDestinations fd ON p.Id = fd.PassengerId
WHERE SUBSTRING(p.FullName, 2, 1) = 'a'
 GROUP BY p.FullName
 HAVING COUNT(AircraftId) > 1
 ORDER BY p.FullName

 --10. Full Info for Flight Destinations

 SELECT a.AirportName, 
		fd.Start AS DayTime, 
		fd.TicketPrice, 
		p.FullName, 
		ac.Manufacturer, 
		ac.Model 
   FROM FlightDestinations fd
   LEFT JOIN Airports a ON fd.AirportId = a.Id
   LEFT JOIN Aircraft ac ON fd.AircraftId = ac.Id
   LEFT JOIN Passengers p ON fd.PassengerId = p.Id
   WHERE DATEPART(HOUR, (fd.[Start])) BETWEEN 6 AND 20 AND fd.TicketPrice > 2500
   ORDER BY ac.Model

 --11. Find all Destinations by Email Address

 GO

 CREATE OR ALTER FUNCTION [udf_FlightDestinationsByEmail](@email VARCHAR(50))
		 RETURNS INT
                      AS 
                   BEGIN
						RETURN (SELECT COUNT(AircraftId) CountOfAircraft
							 FROM Passengers p
						LEFT JOIN FlightDestinations fd ON p.Id = fd.PassengerId
						WHERE p.Email = @email
						 GROUP BY p.FullName)

				     END
GO

SELECT dbo.udf_FlightDestinationsByEmail ('PierretteDunmuir@gmail.com')

SELECT dbo.udf_FlightDestinationsByEmail('Montacute@gmail.com')

SELECT dbo.udf_FlightDestinationsByEmail('MerisShale@gmail.com')

GO

--12. Full Info for Airports

CREATE OR ALTER PROC usp_SearchByAirportName(@airportName VARCHAR(70))
AS
BEGIN
	SELECT  a.AirportName,
			p.FullName,
			CASE
				WHEN fd.TicketPrice <= 400 THEN 'Low'
				WHEN fd.TicketPrice BETWEEN 401 AND 1500 THEN 'Medium'
				WHEN fd.TicketPrice > 1501 THEN 'High'
			END LevelOfTickerPrice,
			ac.Manufacturer,
			ac.Condition,
			[at].TypeName
	  FROM Airports a
 LEFT JOIN FlightDestinations fd ON a.Id = fd.AirportId
 LEFT JOIN Passengers p ON fd.PassengerId = p.Id
 LEFT JOIN Aircraft ac ON fd.AircraftId = ac.Id
 LEFT JOIN AircraftTypes [at] ON ac.TypeId = [at].Id
 WHERE a.AirportName = @airportName
 ORDER BY ac.Manufacturer, p.FullName


END

EXEC usp_SearchByAirportName 'Sir Seretse Khama International Airport'