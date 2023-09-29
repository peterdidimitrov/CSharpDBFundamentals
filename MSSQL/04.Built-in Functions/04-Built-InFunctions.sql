--1.	Find Names of All Employees by First Name
--Create a SQL query that finds all employees whose first name starts with "Sa". As a result, display "FirstName" and "LastName".


SELECT [FirstName]
      ,[LastName]
  FROM [Employees]
 WHERE [FirstName] LIKE 'Sa%'

-- 2.	Find Names of All Employees by Last Name 
--Create a SQL query that finds all employees whose last name contains "ei". As a result, display "FirstName" and "LastName".

SELECT [FirstName]
      ,[LastName]
  FROM [Employees]
 WHERE [LastName] LIKE '%ei%'

-- 3.	Find First Names of All Employees
--Create a SQL query that finds the first names of all employees whose department ID is 3 or 10, and the hire year is between 1995 and 2005 inclusive.

SELECT [FirstName]
  FROM [Employees] 
 WHERE [DepartmentID] = 3 OR [DepartmentID] = 10 AND YEAR ([HireDate]) BETWEEN  1995 AND 2005

-- 4.	Find All Employees Except Engineers
--Create a SQL query that finds the first and last names of every employee, whose job title does not contain "engineer". 

SELECT [FirstName]
      ,[LastName]
  FROM [Employees]
 WHERE [JobTitle] NOT LIKE '%engineer%'

-- 5.	Find Towns with Name Length
--Create a SQL query that finds all town names, which are 5 or 6 symbols long. Order the result alphabetically by town name.  

SELECT [Name]
  FROM [Towns]
 WHERE LEN([Name]) BETWEEN 5 AND 6
 ORDER BY [Name]

-- 6.	Find Towns Starting With
--Create a SQL query that finds all towns with names starting with 'M', 'K', 'B' or 'E'. Order the result alphabetically by town name. 

  SELECT [TownId]
	     ,[Name]
    FROM [Towns]
   WHERE [Name] LIKE 'M%' OR [Name] LIKE 'K%' OR [Name] LIKE 'B%' OR [Name] LIKE 'E%'
ORDER BY [Name]

-- 7.	Find Towns Not Starting With
--Create a SQL query that finds all towns, which do not start with 'R', 'B' or 'D'. Order the result alphabetically by name. 

  SELECT [TownId]
	     ,[Name]
    FROM [Towns]
   WHERE [Name] NOT LIKE 'R%' AND [Name] NOT LIKE 'B%' AND [Name] NOT LIKE 'D%'
ORDER BY [Name]


--8.	Create View Employees Hired After 2000 Year
--Create a SQL query that creates view "V_EmployeesHiredAfter2000" with the first and the last name for all employees, hired after the year 2000. 

CREATE VIEW V_EmployeesHiredAfter2000 AS
	 SELECT [FirstName]
		    ,[LastName]
	   FROM [Employees]
      WHERE YEAR([HireDate]) > 2000;

--9.	Length of Last Name
--Create a SQL query that finds all employees, whose last name is exactly 5 characters long.

SELECT [FirstName]
	   ,[LastName]
  FROM [Employees]
 WHERE LEN([LastName]) = 5

-- 10.	Rank Employees by Salary
--Write a query that ranks all employees using DENSE_RANK. In the DENSE_RANK function, employees need to be partitioned by Salary and ordered by EmployeeID. You need to find only the employees, whose Salary is between 10000 and 50000 and order them by Salary in descending order.

   SELECT [EmployeeID]
          ,[FirstName]
	      ,[LastName]
		  ,[Salary]
		  ,DENSE_RANK() OVER 
		  (PARTITION BY [Salary] ORDER BY [EmployeeID]) AS [Rank]
     FROM [Employees] 
    WHERE [Salary] BETWEEN 10000 AND 50000
 ORDER BY [Salary] DESC

-- 11.	Find All Employees with Rank 2
--Upgrade the query from the previous problem, so that it finds only the employees with a Rank 2. Order the result by Salary (descending).

   SELECT *
   FROM (  SELECT [EmployeeID]
				  ,[FirstName]
				  ,[LastName]
				  ,[Salary]
				  ,DENSE_RANK() 
							OVER (PARTITION BY [Salary] ORDER BY [EmployeeID]) AS [Rank]
			FROM [Employees]
		   WHERE [Salary] BETWEEN 10000 AND 50000
		   ) AS TempRankTable
   WHERE [Rank] = 2
ORDER BY [Salary] DESC

--12.	Countries Holding 'A' 3 or More Times
--Find all countries which hold the letter 'A' at least 3 times in their name (case-insensitively). Sort the result by ISO code and display the "Country Name" and "ISO Code". 

    SELECT [CountryName]
           ,[IsoCode]
      FROM [Countries]
     WHERE [CountryName] Like '%A%A%A%'
  ORDER BY [IsoCode]


--  13. Mix of Peak and River Names
--Combine all peak names with all river names, so that the last letter of each peak name is the same as the first letter of its corresponding river name. Display the peak names, river names and the obtained mix (mix should be in lowercase). Sort the results by the obtained mix.

-- first solution
    SELECT [PeakName]
           ,[RiverName]
           ,LOWER(CONCAT([P].[PeakName], SUBSTRING(R.[RiverName], 2, LEN([R].[RiverName]) - 1)))
           AS [Mix]
     FROM [Peaks] AS [P]
          ,[Rivers] AS [R]
    WHERE RIGHT([PeakName], 1) = LEFT([RiverName], 1)
 ORDER BY [Mix]

--14.	Games from 2011 and 2012 Year
--Find and display the top 50 games which start date is from 2011 and 2012 year. Order them by start date, then by name of the game. The start date should be in the following format: "yyyy-MM-dd". 

SELECT TOP 50 [Name]
              ,FORMAT([Start], 'yyyy-MM-dd') AS [Start]
         FROM [Games] AS [G]
		WHERE YEAR([Start]) BETWEEN 2011 AND 2012
	 ORDER BY [Start], [Name]

--15.	 User Email Providers
--Find all users with information about their email providers. Display the username and email provider. Sort the results by email provider alphabetically, then by username. 

SELECT [Username]
	   ,SUBSTrING([Email], (CHARINDEX('@', [Email], 1) + 1), LEN([Email]) - CHARINDEX('@', [Email], 1)) AS [Email Providers]
FROM  [Users]
ORDER BY [Email Providers], [Username]

--16.	 Get Users with IP Address Like Pattern
--Find all users with their IP addresses, sorted by username alphabetically. Display only the rows which IP address matches the pattern: "***.1^.^.***". 
--Legend: 
--* - one symbol
--^ - one or more symbols

SELECT [Username]
	   ,[IpAddress]
FROM  [Users]
WHERE [IpAddress] LIKE '___.1_%._%.___'
ORDER BY [Username]

--17.	 Show All Games with Duration and Part of the Day
--Find all games with part of the day and duration. Sort them by game name alphabetically, then by duration (alphabetically, not by the timespan) and part of the day (all ascending). Part of the Day should be Morning (time is >= 0 and < 12), Afternoon (time is >= 12 and < 18), Evening (time is >= 18 and < 24). Duration should be Extra Short (smaller or equal to 3), Short (between 4 and 6 including), Long (greater than 6) and Extra Long (without duration). 

   SELECT [Name] AS [Game]
	      ,CASE
			   WHEN DATEPART(HOUR, [Start]) >= 0 AND DATEPART(HOUR, [Start]) < 12 THEN 'Morning'
			   WHEN DATEPART(HOUR, [Start]) >= 12 AND DATEPART(HOUR, [Start]) < 18 THEN 'Afternoon'
			   ELSE 'Evening'
	      END AS [Part of the Day]
	      ,CASE
			   WHEN [Duration] <= 3 THEN 'Extra Short'
			   WHEN [Duration] BETWEEN 4 AND 6 THEN 'Short'
			   WHEN [Duration] > 6 THEN 'Long'
			   ELSE 'Extra Long'
	      END AS [Duration]
     FROM [Games]
 ORDER BY [Game], [Duration], [Part of the Day]

--18. Orders Table
--You are given a table Orders(Id, ProductName, OrderDate) filled with data. Consider that the payment for that order must be accomplished within 3 days after the order date. Also the delivery date is up to 1 month. Write a query to show each product's name, order date, pay and deliver due dates. 

SELECT [ProductName]
	   ,[OrderDate]
	   ,DATEADD(DAY,3,OrderDate) AS [Pay Due]
	   ,DATEADD(MONTH,1,OrderDate) AS [Deliver Due]
     FROM [Orders]

--	 19.	 People Table
--Create a table People(Id, Name, Birthdate). Write a query to find age in years, months, days and minutes for each person for the current time of executing the query.

CREATE DATABASE [TEST]
   CREATE TABLE [People] (
			    [Id] INT PRIMARY KEY IDENTITY
				,[Name] NVARCHAR(50) NOT NULL
				,[Birthdate] DATETIME2 NOT NULL
   )

   INSERT INTO [People] ([Name], [Birthdate])
   VAlUES ('Victor', '2000-12-07 00:00:00.000'),
   ('Steven', '1992-09-10 00:00:00.000'),
   ('Stephen', '1910-09-19 00:00:00.000'),
   ('John', '2010-01-06 00:00:00.000')

   SELECT [Name]
	      ,DATEDIFF(YEAR, [Birthdate], GETDATE()) AS [Age in Years]
		  ,DATEDIFF(MONTH, [Birthdate], GETDATE()) AS [Age in Months]
		  ,DATEDIFF(DAY, [Birthdate], GETDATE()) AS [Age in Days]
		  ,DATEDIFF(MINUTE, [Birthdate], GETDATE()) AS [Age in Minutes]
  FROM [People]