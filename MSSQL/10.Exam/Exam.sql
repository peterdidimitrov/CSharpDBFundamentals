

--1


CREATE DATABASE TouristAgency
USE TouristAgency

CREATE TABLE Countries (
		Id INT PRIMARY KEY IDENTITY,
		[Name] NVARCHAR(50) NOT NULL,
)


CREATE TABLE Destinations (
		Id INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(50) NOT NULL,
		CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL,
)


CREATE TABLE Rooms (
		Id INT PRIMARY KEY IDENTITY,
		[Type] VARCHAR(40) NOT NULL,
		Price DECIMAL(18,2) NOT NULL,
		BedCount INT CHECK (BedCount > 0 AND BedCount <= 10 ) NOT NULL
)


CREATE TABLE Hotels (
		Id INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(50) NOT NULL,
		DestinationId INT FOREIGN KEY REFERENCES Destinations(Id) NOT NULL
)


CREATE TABLE Tourists (
		Id INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(80) NOT NULL,
		PhoneNumber VARCHAR(20) NOT NULL,
		Email VARCHAR(80),
		CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
)


CREATE TABLE Bookings (
		Id INT PRIMARY KEY IDENTITY,
		ArrivalDate DateTime2 NOT NULL,
		DepartureDate DateTime2 NOT NULL,
		AdultsCount INT CHECK(AdultsCount BETWEEN 1 AND 10) NOT NULL,
		ChildrenCount INT CHECK(ChildrenCount BETWEEN 0 AND 9) NOT NULL,
		TouristId INT FOREIGN KEY REFERENCES Tourists(Id) NOT NULL,
		HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL,
		RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL
)


CREATE TABLE HotelsRooms (
		HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL, 
		RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL
		PRIMARY KEY (HotelId, RoomId)
)

--02. Insert

INSERT INTO Tourists([Name], PhoneNumber, Email, CountryId) VALUES
		('John Rivers',	'653-551-1555',	'john.rivers@example.com',	6),
		('Adeline Aglaé',	'122-654-8726',	'adeline.aglae@example.com',	2),
		('Sergio Ramirez', '233-465-2876', 's.ramirez@example.com', 3),
		('Johan Müller', '322-876-9826', 'j.muller@example.com', 7),
		('Eden Smith', '551-874-2234', 'eden.smith@example.com', 6)

INSERT INTO Bookings(ArrivalDate, DepartureDate, AdultsCount, ChildrenCount, TouristId, HotelId, RoomId) VALUES
		('2024-03-01', '2024-03-11', 1, 0, 21, 3, 5),
		('2023-12-28', '2024-01-06', 2, 1, 22, 13, 5),
		('2023-11-15', '2023-11-20', 1, 2, 23, 19, 7)

--03. Update

UPDATE Bookings
   SET DepartureDate = DATEADD(day, 1, DepartureDate)
 WHERE MONTH(DepartureDate) = 12 AND YEAR(DepartureDate) = 2023

 UPDATE Tourists
   SET Email = NULL
 WHERE [Name] LIKE '%MA%'

-- 04. Delete

DELETE FROM Bookings 
WHERE TouristId IN (SELECT Id FROM Tourists WHERE [Name] LIKE '%Smith')

DELETE FROM Tourists
WHERE [Name] LIKE '%Smith'



--5


SELECT FORMAT(ArrivalDate, 'yyyy-MM-dd'),
	   AdultsCount,
	   ChildrenCount
FROM Bookings b
LEFT JOIN Rooms r ON b.RoomId = r.Id
ORDER BY r.Price DESC, b.ArrivalDate


--6

SELECT h.Id,
	   h.[Name]
FROM Hotels AS h
LEFT JOIN HotelsRooms AS hr ON h.Id = hr.HotelId
LEFT JOIN Rooms AS r ON hr.RoomId = r.[Id]
LEFT JOIN Bookings b ON h.Id = b.HotelId
WHERE r.Type = 'VIP Apartment'
GROUP BY h.Id, h.[Name]
ORDER BY COUNT(h.Id) DESC

--7

SELECT t.Id,
	   t.[Name],
	   t.PhoneNumber
FROM Tourists t
LEFT JOIN Bookings AS b ON t.Id = b.TouristId
WHERE b.TouristId IS NULL
ORDER BY t.[Name]


--8

SELECT TOP 10 h.Name HotelName, d.[Name] DestinationName, c.[Name] CountryName
FROM Bookings AS b
LEFT JOIN Hotels AS h ON b.HotelId = h.Id
LEFT JOIN Tourists AS t ON b.TouristId = t.Id
LEFT JOIN Destinations AS d ON h.DestinationId = d.Id
LEFT JOIN Countries c ON d.CountryId = c.Id
WHERE b.ArrivalDate < '2023-12-31' AND h.Id % 2 = 1
ORDER BY c.[Name], b.ArrivalDate


--9

SELECT h.[Name] HotelName, r.Price RoomPrice
FROM Tourists t
INNER JOIN Bookings AS b ON t.Id = b.TouristId
INNER JOIN Hotels AS h ON h.Id = b.HotelId
INNER JOIN Rooms AS r ON b.RoomId = r.Id
WHERE t.[Name] NOT LIKE '%EZ'
ORDER BY r.Price DESC


--10

SELECT h.[Name] HotelName
	   ,SUM(r.Price * DATEDIFF(day, b.ArrivalDate, b.DepartureDate)) HotelRevenue
FROM Bookings b
LEFT JOIN Hotels AS h ON b.HotelId = h.Id
LEFT JOIN Rooms AS r ON b.RoomId = r.Id
GROUP BY h.[Name]
ORDER BY HotelRevenue DESC


--11

CREATE OR ALTER FUNCTION [udf_RoomsWithTourists] (@name VARCHAR(40))
		 RETURNS INT
                      AS 
                   BEGIN
						RETURN (SELECT SUM(b.AdultsCount) +
										SUM(b.ChildrenCount)
											FROM Bookings AS b
											LEFT JOIN Rooms AS r ON b.RoomId = r.Id
											WHERE r.[Type] = @name)

				     END

GO

SELECT dbo.udf_RoomsWithTourists('Double Room')

GO


--12

CREATE PROC usp_SearchByCountry(@country NVARCHAR(50))
AS
BEGIN

SELECT t.[Name]
	   ,t.PhoneNumber
	   ,t.Email 
	   ,COUNT(b.Id) CountOfBookings
FROM Tourists t
LEFT JOIN Bookings b ON t.Id = b.TouristId
LEFT JOIN Countries c ON t.CountryId = c.Id
WHERE c.[Name] = @country
GROUP BY t.[Name], t.PhoneNumber, t.Email

END

EXEC usp_SearchByCountry 'Greece'

GO

