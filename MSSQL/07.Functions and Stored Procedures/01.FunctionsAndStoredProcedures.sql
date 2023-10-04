--1.	Employees with Salary Above 35000
--Create stored procedure usp_GetEmployeesSalaryAbove35000 that returns all employees' first and last names, whose salary above 35000. 


CREATE PROCEDURE [usp_GetEmployeesSalaryAbove35000]
--CREATE OR ALTER PROCEDURE [usp_GetEmployeesSalaryAbove35000]
			  AS
			  BEGIN
					SELECT [FirstName]
						   ,[LastName]
					  FROM [Employees]
					 WHERE [Salary] > 35000
			  END

EXECUTE [dbo].[usp_GetEmployeesSalaryAbove35000]
GO

--2.	Employees with Salary Above Number
--Create a stored procedure usp_GetEmployeesSalaryAboveNumber that accepts a number (of type DECIMAL(18,4)) as parameter and returns all employees' first and last names, whose salary is above or equal to the given number. 

CREATE PROCEDURE [usp_GetEmployeesSalaryAboveNumber] @minSalary DECIMAL(18, 4)
			  AS
			  BEGIN
					SELECT [FirstName]
						   ,[LastName]
					  FROM [Employees]
					 WHERE [Salary] >= @minSalary
			  END

EXECUTE [dbo].[usp_GetEmployeesSalaryAboveNumber] 48100

GO

--3.	Town Names Starting With
--Create a stored procedure usp_GetTownsStartingWith that accepts a string as parameter and returns all town names starting with that string. 


CREATE PROCEDURE [usp_GetTownsStartingWith] @letter NVARCHAR(50)
			  AS
			  BEGIN
					SELECT [Name] 
						AS [Town]
					  FROM [Towns]
					 WHERE LOWER(LEFT([Name],LEN(@letter))) = LOWER(@letter)
			  END

EXECUTE [dbo].[usp_GetTownsStartingWith] 'b'

GO

--4.	Employees from Town
--Create a stored procedure usp_GetEmployeesFromTown that accepts town name as parameter and returns the first and last name of those employees, who live in the given town. 

CREATE OR ALTER PROCEDURE [usp_GetEmployeesFromTown] @townName NVARCHAR(50)
			  AS
			  BEGIN
					SELECT [FirstName] AS [First Name]
						   ,[LastName] AS  [Last Name]
					  FROM [Employees]
					    AS [e]
				INNER JOIN [Addresses] AS [a] ON [e].[AddressID] = [a].[AddressID]
				INNER JOIN [Towns] AS [t] ON [a].[TownID] = [t].[TownID]
					 WHERE LOWER([t].[Name]) = LOWER(@townName)
			  END

EXECUTE [dbo].[usp_GetEmployeesFromTown] 'Sofia'

GO

--5.	Salary Level Function
--Create a function ufn_GetSalaryLevel(@salary DECIMAL(18,4)) that receives salary of an employee and returns the level of the salary.
--•	If salary is < 30000, return "Low"
--•	If salary is between 30000 and 50000 (inclusive), return "Average"
--•	If salary is > 50000, return "High"


CREATE OR ALTER FUNCTION [ufn_GetSalaryLevel](@salary DECIMAL(18,4))
		 RETURNS VARCHAR (8)
                      AS 
                   BEGIN
						DECLARE @salaryLevel VARCHAR (8)
						IF @salary < 30000
						BEGIN
								SET @salaryLevel = 'Low'
						END
						ELSE IF @salary BETWEEN 30000 AND 50000 
						BEGIN
								SET @salaryLevel = 'Average'
						END
						ELSE IF @salary > 50000 
						BEGIN
								SET @salaryLevel = 'High'
						END
						RETURN @salaryLevel
				     END

GO

--6.	Employees by Salary Level
--Create a stored procedure usp_EmployeesBySalaryLevel that receives as parameter level of salary (low, average, or high) and print the names of all employees, who have the given level of salary. You should use the function - "dbo.ufn_GetSalaryLevel(@Salary)", which was part of the previous task, inside your "CREATE PROCEDURE …" query.


CREATE OR ALTER PROCEDURE [usp_EmployeesBySalaryLevel] @levelOfSalary VARCHAR (8)
					   AS
					BEGIN
						SELECT [FirstName] AS [First Name]
							   ,[LastName] AS [Last Name]
						  FROM [Employees]
							AS [e]
						 WHERE LOWER([dbo].[ufn_GetSalaryLevel]([Salary])) = LOWER(@levelOfSalary)
					  END

EXECUTE [dbo].[usp_EmployeesBySalaryLevel] 'high'


GO

--7.	Define Function
--Define a function ufn_IsWordComprised(@setOfLetters, @word) that returns true or false, depending on that if the word is comprised of the given set of letters. 


CREATE OR ALTER FUNCTION [ufn_IsWordComprised](@setOfLetters VARCHAR(50), @word VARCHAR(50))
		     RETURNS BIT
                      AS 
                   BEGIN
							DECLARE @wordIndex INT = 1
							WHILE (@wordIndex <= LEN(@word))
							BEGIN
								DECLARE @currentCharacter CHAR = SUBSTRING(@word, @wordIndex, 1)
									 IF CHARINDEX(@currentCharacter, @setOfLetters) = 0
										BEGIN
											RETURN 0;
										END
									SET @wordIndex += 1;
							  END
						   RETURN 1;
				     END

					  GO

SELECT [dbo].[ufn_IsWordComprised]('oistmiahf', 'halves')

	GO

--8.	Delete Employees and Departments
--Create a procedure with the name usp_DeleteEmployeesFromDepartment (@departmentId INT) which deletes all Employees from a given department. Delete these departments from the Departments table too. Finally, SELECT the number of employees from the given department. If the delete statements are correct the select query should return 0.
--After completing that exercise restore your database to revert all changes.
--Hint:
--You may set ManagerID column in Departments table to nullable (using query "ALTER TABLE …").


CREATE OR ALTER PROCEDURE [usp_DeleteEmployeesFromDepartment] @departmentId INT
					   AS
					BEGIN

				  DECLARE @employeesToDelete TABLE ([Id] INT) 
			  INSERT INTO @employeesToDelete
							 SELECT [EmployeeID] 
							   FROM [Employees]
							  WHERE [DepartmentID] = @departmentId

				   DELETE
					 FROM [EmployeesProjects]
					WHERE [EmployeeID] IN (SELECT * FROM @employeesToDelete)

			  ALTER TABLE [Departments]
			 ALTER COLUMN [ManagerID] INT
				
				   UPDATE [Departments]
				      SET [ManagerID] = NULL
					WHERE [ManagerID] IN (SELECT * FROM @employeesToDelete)

				   UPDATE [Employees]
				      SET [ManagerID] = NULL
					WHERE [ManagerID] IN (SELECT * FROM @employeesToDelete)

			       DELETE
					 FROM [Employees]
					WHERE [DepartmentID] = @departmentId

				   DELETE
					 FROM [Departments]
					WHERE [DepartmentID] = @departmentId

				   SELECT COUNT(*) AS [Count]
					 FROM [Employees]
					WHERE [DepartmentID] = @departmentId

					  END


				  EXECUTE [dbo].[usp_DeleteEmployeesFromDepartment] 1

					   GO

				   SELECT *
					 FROM [Employees]
				    WHERE [DepartmentID] = 1
					
					   GO

--9.	Find Full Name
--You are given a database schema with tables AccountHolders(Id (PK), FirstName, LastName, SSN) and Accounts(Id (PK), AccountHolderId (FK), Balance).  Write a stored procedure usp_GetHoldersFullName that selects the full name of all people. 


CREATE OR ALTER PROCEDURE [usp_GetHoldersFullName]
					   AS
					BEGIN

				   SELECT CONCAT_WS(' ', [FirstName], [LastName]) AS [Full Name]
					 FROM [AccountHolders]

					  END

				  EXECUTE [dbo].[usp_GetHoldersFullName]
				       GO

--10.	People with Balance Higher Than
--Your task is to create a stored procedure usp_GetHoldersWithBalanceHigherThan that accepts a number as a parameter and returns all the people, who have more money in total in all their accounts than the supplied number. Order them by their first name, then by their last name.


CREATE OR ALTER PROCEDURE [usp_GetHoldersWithBalanceHigherThan] @number MONEY
					   AS
					BEGIN

				   SELECT [ah].[FirstName] AS [First Name]
				          ,[ah].[LastName] AS [Last Name]
					 FROM [AccountHolders] As [ah]
			   RIGHT JOIN [Accounts] AS [a] ON [ah].[Id] = [a].[AccountHolderId]
			     GROUP BY [ah].[FirstName], [ah].[LastName]
				   HAVING SUM([a].[Balance]) > @number
				 ORDER BY [ah].[FirstName], [ah].[LastName]

					  END

				  EXECUTE [dbo].[usp_GetHoldersWithBalanceHigherThan] 50000
				  GO

--11	Future Value Function
--Your task is to create a function ufn_CalculateFutureValue that accepts as parameters – sum (decimal), yearly interest rate (float), and the number of years (int). It should calculate and return the future value of the initial sum rounded up to the fourth digit after the decimal delimiter. Use the following formula:
--FV=I×(〖(1+R)〗^T)
--	I – Initial sum
--	R – Yearly interest rate
--	T – Number of years

CREATE OR ALTER FUNCTION [ufn_CalculateFutureValue](@sum DECIMAL(38, 4), @rate FLOAT, @years INT)
		     RETURNS DECIMAL(38, 4)
                      AS 
                   BEGIN
						    RETURN @sum * POWER((1 + @rate), @years) 
				     END

					 GO

SELECT [dbo].[ufn_CalculateFutureValue](1000, 0.1, 5)

	GO

--12.	Calculating Interest
--Your task is to create a stored procedure usp_CalculateFutureValueForAccount that uses the function from the previous problem to give an interest to a person's account for 5 years, along with information about their account id, first name, last name and current balance as it is shown in the example below. It should take the AccountId and the interest rate as parameters. Again, you are provided with the dbo.ufn_CalculateFutureValue function, which was part of the previous task.


CREATE OR ALTER PROCEDURE [usp_CalculateFutureValueForAccount] (@accountId  INT, @rate FLOAT)
					   AS
					BEGIN

				   SELECT [a].[Id] AS [Account Id]
						  ,[ah].[FirstName] AS [First Name]
				          ,[ah].[LastName] AS [Last Name]
						  ,[a].[Balance] AS [Current Balance]
						  ,[dbo].[ufn_CalculateFutureValue]([Balance], @rate, 5) AS [Balance in 5 years]
					 FROM [AccountHolders] As [ah]
			         JOIN [Accounts] AS [a] ON [ah].[Id] = [a].[Id]
					WHERE [a].[Id] = @accountId

					  END

				  EXECUTE [dbo].[usp_CalculateFutureValueForAccount] 1, 0.1
				  GO


--13.	*Scalar Function: Cash in User Games Odd Rows
--Create a function ufn_CashInUsersGames that sums the cash of the odd rows. Rows must be ordered by cash in descending order. The function should take a game name as a parameter and return the result as a table. Submit only your function in.
--Execute the function over the following game names, ordered exactly like: "Love in a mist".
