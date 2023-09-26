--1.	Employee Address
--Create a query that selects:
--•	EmployeeId
--•	JobTitle
--•	AddressId
--•	AddressText
--Return the first 5 rows sorted by AddressId in ascending order.

SELECT TOP 5 [EmployeeID]
			 ,[JobTitle]
			 ,[e].[AddressId]
			 ,[a].[AddressText]
	    FROM [Employees] AS [e]
		JOIN [Addresses] AS [a] ON [e].[AddressID] = [a].[AddressID]
	ORDER BY [AddressID]

--2.	Addresses with Towns
--Write a query that selects:
--•	FirstName
--•	LastName
--•	Town
--•	AddressText
--Sort them by FirstName in ascending order, then by LastName. Select the first 50 employees.

SELECT TOP 50 [FirstName]
			 ,[LastName]
			 ,[t].[Name] AS [Town]
			 ,[a].[AddressText]
	    FROM [Employees] AS [e]
		JOIN [Addresses] AS [a] ON [e].[AddressID] = [a].[AddressID]
		JOIN [Towns] AS [t] ON [a].[TownID] = [t].[TownID]
	ORDER BY [FirstName], [LastName]

--	3.	Sales Employee
--Create a query that selects:
--•	EmployeeID
--•	FirstName
--•	LastName
--•	DepartmentName
--Sort them by EmployeeID in ascending order. Select only employees from the "Sales" department.


   SELECT [EmployeeID]
		 ,[FirstName]
	     ,[LastName]
	     ,[d].[Name] AS [DepartmentName]
    FROM [Employees] AS [e]
    JOIN [Departments] AS [d] ON [e].[DepartmentID] = [d].[DepartmentID]
   WHERE [d].[Name] = 'Sales'

-- 4.	Employee Departments
--Create a query that selects:
--•	EmployeeID
--•	FirstName 
--•	Salary
--•	DepartmentName
--Filter only employees with a salary higher than 15000. Return the first 5 rows, sorted by DepartmentID in ascending order.

  SELECT TOP 5 [EmployeeID]
			   ,[FirstName]
			   ,[Salary]
			   ,[d].[Name] AS [DepartmentName]
		  FROM [Employees] AS [e]
		  JOIN [Departments] AS [d] ON [e].[DepartmentID] = [d].[DepartmentID]
	     WHERE [Salary] > 15000
	  ORDER BY [e].[DepartmentID]

--5.	Employees Without Project
--Create a query that selects:
--•	EmployeeID
--•	FirstName
--Filter only employees without a project. Return the first 3 rows, sorted by EmployeeID in ascending order.

  SELECT TOP 3 [e].[EmployeeID]
			   ,[FirstName]
		  FROM [Employees] AS [e]
		  FULL JOIN [EmployeesProjects] AS [ep] ON [e].[EmployeeID] = [ep].[EmployeeID]
	     WHERE [ep].[EmployeeID] IS NULL
	  ORDER BY [e].[EmployeeID]

--6.	Employees Hired After
--Create a query that selects:
--•	FirstName
--•	LastName
--•	HireDate
--•	DeptName
--Filter only employees hired after 1.1.1999 and are from either "Sales" or "Finance" department. Sort them by HireDate (ascending).

        SELECT [FirstName]
			   ,[LastName]
			   ,[HireDate]
			   ,[d].[Name] AS [DeptName]
		  FROM [Employees] AS [e]
		  FULL JOIN [Departments] AS [d] ON [e].[DepartmentID] = [d].[DepartmentID]
	     WHERE [d].[Name] = 'Sales' OR [d].[Name] = 'Finance'
	  ORDER BY [e].[HireDate]


--7.	Employees with Project
--Create a query that selects:
--•	EmployeeID
--•	FirstName
--•	ProjectName
--Filter only employees with a project which has started after 13.08.2002 and it is still ongoing (no end date). Return the first 5 rows sorted by EmployeeID in ascending order.

  SELECT TOP 5 [e].[EmployeeID]
			   ,[FirstName]
			   ,[p].[Name] AS [ProjectName]
		  FROM [Employees] AS [e]
		  JOIN [EmployeesProjects] AS [ep] ON [e].[EmployeeID] = [ep].[EmployeeID]
		  JOIN [Projects] AS [p] ON [ep].[ProjectID] = [p].[ProjectID]
	     WHERE  [p].[StartDate] > '2002-08-13' AND [p].[EndDate] IS NULL
	  ORDER BY [e].[EmployeeID]

--8.	Employee 24
--Create a query that selects:
--•	EmployeeID
--•	FirstName
--•	ProjectName
--Filter all the projects of employee with Id 24. If the project has started during or after 2005 the returned value should be NULL.

        SELECT [ep].[EmployeeID]
			   ,[e].[FirstName]
			   ,CASE
					WHEN [StartDate] >= '2005-01-01' THEN NULL
					ELSE [p].[Name]
					END
				AS [ProjectName]
		  FROM [EmployeesProjects] AS [ep]
		  JOIN [Employees] AS [e] ON [e].[EmployeeID] = [ep].[EmployeeID]
		  JOIN [Projects] AS [p] ON [ep].[ProjectID] = [p].[ProjectID]
	     WHERE [ep].[EmployeeID] = 24

--9.	Employee Manager
--Create a query that selects:
--•	EmployeeID
--•	FirstName
--•	ManagerID
--•	ManagerName
--Filter all employees with a manager who has ID equals to 3 or 7. Return all the rows, sorted by EmployeeID in ascending order.

		SELECT [e].[EmployeeID]
			   ,[e].[FirstName]
			   ,[e].[ManagerID]
			   ,[m].[FirstName] AS [ManagerName]
		  FROM [Employees] AS [e]
	INNER JOIN [Employees] AS [m] ON [e].[ManagerID] = [m].[EmployeeID]
	     WHERE [e].[ManagerID] IN (3, 7)
	  ORDER BY [EmployeeID]
