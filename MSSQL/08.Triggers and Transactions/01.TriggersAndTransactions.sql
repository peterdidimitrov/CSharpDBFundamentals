--1.	Create Table Logs
--Create a table – Logs (LogId, AccountId, OldSum, NewSum). Add a trigger to the Accounts table that enters a new entry into the Logs table every time the sum on an account change. Submit only the query that creates the trigger.

USE [Bank]

CREATE TABLE [Logs](
	[LogId] INT NOT NULL IDENTITY
	,[AccountId] INT FOREIGN KEY REFERENCES [Accounts]([Id])
	,[OldSum] MONEY
	,[NewSum] MONEY
)
GO

CREATE OR ALTER TRIGGER [dbo].[trg_addNewSum]
					 ON [dbo].[Accounts]
                  AFTER UPDATE
                     AS 
				  BEGIN
						INSERT INTO [Logs]([AccountID], [OldSum], [NewSum])
							 SELECT [i].[Id], [d].[Balance], [i].[Balance] 
							   FROM inserted AS [i]
						 INNER JOIN deleted AS [d]
								 ON [i].[Id] = [d].[Id]
					END

					GO


--2.	Create Table Emails
--Create another table – NotificationEmails(Id, Recipient, Subject, Body). Add a trigger to logs table and create new email whenever new record is inserted in logs table. The following data is required to be filled for each email:
--•	Recipient – AccountId
--•	Subject – "Balance change for account: {AccountId}"
--•	Body - "On {date} your balance was changed from {old} to {new}."
--Submit your query only for the trigger action.


CREATE TABLE [NotificationEmails](
	[Id] INT NOT NULL IDENTITY
	,[Recipient] INT FOREIGN KEY REFERENCES [Accounts]([Id])
	,[Subject] NVARCHAR(MAX)
	,[Body] NVARCHAR(MAX)
)

GO

CREATE OR ALTER TRIGGER [dbo].[trg_Notification]
					 ON [dbo].[Logs]
                  AFTER INSERT
                     AS 
				  BEGIN
						INSERT INTO [NotificationEmails]([Recipient], [Subject], [Body])
							 VALUES ( 
							   (SELECT [AccountId]
							      FROM [inserted])
							   ,(CONCAT('Balance change for account: ', (SELECT [AccountId] FROM [inserted])))
							   ,(CONCAT('On ', (SELECT GETDATE() FROM [inserted]), ' your balance was changed from ', (SELECT [OldSum] FROM [inserted]), ' to ', (SELECT [NewSum] FROM [inserted]), '.'))
									)
					END

					GO

--3.	Deposit Money
--Add stored procedure usp_DepositMoney(AccountId, MoneyAmount) that deposits money to an existing account. Make sure to guarantee valid positive MoneyAmount with precision up to the fourth sign after the decimal point. The procedure should produce exact results working with the specified precision.

CREATE OR ALTER PROCEDURE [usp_DepositMoney] @AccountId INT, @MoneyAmount DECIMAL(38,4)
			  AS
			  BEGIN TRANSACTION
					     UPDATE [Accounts]
					        SET [Balance] += @MoneyAmount
					      WHERE [Id] = @AccountId
			             COMMIT

EXECUTE [dbo].[usp_DepositMoney] 1, 10

GO

--4.	Withdraw Money Procedure
--Add stored procedure usp_WithdrawMoney (AccountId, MoneyAmount) that withdraws money from an existing account. Make sure to guarantee valid positive MoneyAmount with precision up to the fourth sign after decimal point. The procedure should produce exact results working with the specified precision.

CREATE OR ALTER PROCEDURE [usp_WithdrawMoney] @AccountId INT, @MoneyAmount DECIMAL(38,4)
			  AS
			  BEGIN TRANSACTION
						IF @MoneyAmount > 0
					     UPDATE [Accounts]
					        SET [Balance] -= @MoneyAmount
					      WHERE [Id] = @AccountId
			             COMMIT

EXECUTE [dbo].[usp_WithdrawMoney] 5, 25

GO

--5.	Money Transfer
--Create stored procedure usp_TransferMoney(SenderId, ReceiverId, Amount) that transfers money from one account to another. Make sure to guarantee valid positive MoneyAmount with precision up to the fourth sign after the decimal point. Make sure that the whole procedure passes without errors and if an error occurs make no change in the database. You can use both: "usp_DepositMoney", "usp_WithdrawMoney" (look at the previous two problems about those procedures). 


CREATE OR ALTER PROCEDURE [usp_TransferMoney] @SenderId INT, @ReceiverId INT, @Amount DECIMAL(38,4)
			  AS
			  BEGIN TRANSACTION
						IF @Amount > 0
					     EXECUTE [dbo].[usp_DepositMoney] @ReceiverId, @Amount
						 EXECUTE [dbo].[usp_WithdrawMoney] @SenderId, @Amount
			             COMMIT

EXECUTE [dbo].[usp_TransferMoney] 5, 1, 5000

GO

--6.	Trigger
--Users should not be allowed to buy items with a higher level than their level. Create a trigger that restricts that. The trigger should prevent inserting items that are above the specified level while allowing all others to be inserted.
--Add bonus cash of 50000 to users: baleremuda, loosenoise, inguinalself, buildingdeltoid, monoxidecos in the game "Bali".
--There are two groups of items that you must buy for the above users. The first are items with id between 251 and 299 including. The second group are items with id between 501 and 539 including.
--Take cash from each user for the bought items.
--Select all users in the current game ("Bali") with their items. Display username, game name, cash and item name. Sort the result by username alphabetically, then by item name alphabetically. 

USE [Diablo]

CREATE OR ALTER TRIGGER [dbo].[trg_buyItem]
					 ON [dbo].[Accounts]
                  AFTER UPDATE
                     AS 
				  BEGIN
						INSERT INTO [Logs]([AccountID], [OldSum], [NewSum])
							 SELECT [i].[Id], [d].[Balance], [i].[Balance] 
							   FROM inserted AS [i]
						 INNER JOIN deleted AS [d]
								 ON [i].[Id] = [d].[Id]
					END

					GO

SELECT [Username]
	  ,[g].[Name]
	  ,[ug].[Cash]
	  ,[i].[Name] AS [Item Name]
	  ,[i].[MinLevel] AS [ItemLevel]
	  ,[ug].[Level] AS [UserLevel]
  FROM [Users] AS [u]
LEFT JOIN [UsersGames] AS [ug] ON [u].[Id] = [ug].[UserId]
LEFT JOIN [UserGameItems] AS [ugi] ON [ug].[Id] = [ugi].[UserGameId]
LEFT JOIN [Items] AS [i] ON [ugi].[ItemId] = [i].[Id]
LEFT JOIN [Games] AS [g] ON [ug].[GameId] = [g].[Id]
WHERE [g].[Name] = 'Bali'
ORDER BY [Username], [Item Name]


--7.	*Massive Shopping
--User Stamat in Safflower game wants to buy some items. He likes all items from Level 11 to 12 as well as all items from Level 19 to 21. As it is a bulk operation you have to use transactions. 
--A transaction is the operation of taking out the cash from the user in the current game as well as adding up the items. 
--Write transactions for each level range. If anything goes wrong turn back the changes inside of the transaction.
--Extract all of Stamat's item names in the given game sorted by name alphabetically.



DECLARE @gameName NVARCHAR(50) = 'Safflower'
DECLARE @username NVARCHAR(50) = 'Stamat'

DECLARE @userGameId INT = (
  SELECT ug.Id
  FROM UsersGames AS ug
    JOIN Users AS u
      ON ug.UserId = u.Id
    JOIN Games AS g
      ON ug.GameId = g.Id
  WHERE u.Username = @username AND g.Name = @gameName)

DECLARE @userGameLevel INT = (SELECT Level
                              FROM UsersGames
                              WHERE Id = @userGameId)
DECLARE @itemsCost MONEY, @availableCash MONEY, @minLevel INT, @maxLevel INT

SET @minLevel = 11
SET @maxLevel = 12
SET @availableCash = (SELECT Cash
                      FROM UsersGames
                      WHERE Id = @userGameId)
SET @itemsCost = (SELECT SUM(Price)
                  FROM Items
                  WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

IF (@availableCash >= @itemsCost AND @userGameLevel >= @maxLevel)

  BEGIN
    BEGIN TRANSACTION
    UPDATE UsersGames
    SET Cash -= @itemsCost
    WHERE Id = @userGameId
    IF (@@ROWCOUNT <> 1)
      BEGIN
        ROLLBACK
        RAISERROR ('Could not make payment', 16, 1)
      END
    ELSE
      BEGIN
        INSERT INTO UserGameItems (ItemId, UserGameId)
          (SELECT
             Id,
             @userGameId
           FROM Items
           WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

        IF ((SELECT COUNT(*)
             FROM Items
             WHERE MinLevel BETWEEN @minLevel AND @maxLevel) <> @@ROWCOUNT)
          BEGIN
            ROLLBACK;
            RAISERROR ('Could not buy items', 16, 1)
          END
        ELSE COMMIT;
      END
  END

SET @minLevel = 19
SET @maxLevel = 21
SET @availableCash = (SELECT Cash
                      FROM UsersGames
                      WHERE Id = @userGameId)
SET @itemsCost = (SELECT SUM(Price)
                  FROM Items
                  WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

IF (@availableCash >= @itemsCost AND @userGameLevel >= @maxLevel)

  BEGIN
    BEGIN TRANSACTION
    UPDATE UsersGames
    SET Cash -= @itemsCost
    WHERE Id = @userGameId

    IF (@@ROWCOUNT <> 1)
      BEGIN
        ROLLBACK
        RAISERROR ('Could not make payment', 16, 1)
      END
    ELSE
      BEGIN
        INSERT INTO UserGameItems (ItemId, UserGameId)
          (SELECT
             Id,
             @userGameId
           FROM Items
           WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

        IF ((SELECT COUNT(*)
             FROM Items
             WHERE MinLevel BETWEEN @minLevel AND @maxLevel) <> @@ROWCOUNT)
          BEGIN
            ROLLBACK
            RAISERROR ('Could not buy items', 16, 1)
          END
        ELSE COMMIT;
      END
  END

SELECT i.Name AS [Item Name]
FROM UserGameItems AS ugi
  JOIN Items AS i
    ON i.Id = ugi.ItemId
  JOIN UsersGames AS ug
    ON ug.Id = ugi.UserGameId
  JOIN Games AS g
    ON g.Id = ug.GameId
WHERE g.Name = @gameName
ORDER BY [Item Name]

GO

--8.	Employees with Three Projects
--Create a procedure usp_AssignProject(@emloyeeId, @projectID) that assigns projects to an employee. If the employee has more than 3 project throw exception and rollback the changes. The exception message must be: "The employee has too many projects!" with Severity = 16, State = 1.

CREATE PROCEDURE usp_AssignProject(@employeeId INT, @projectID INT)
AS
  BEGIN
    BEGIN TRAN
    INSERT INTO EmployeesProjects
    VALUES (@employeeId, @projectID)
    IF (SELECT COUNT(ProjectID)
        FROM EmployeesProjects
        WHERE EmployeeID = @employeeId) > 3
      BEGIN
        RAISERROR ('The employee has too many projects!', 16, 1)
        ROLLBACK
        RETURN
      END
    COMMIT
  END

--  9.	Delete Employees
--Create a table Deleted_Employees(EmployeeId PK, FirstName, LastName, MiddleName, JobTitle, DepartmentId, Salary) that will hold information about fired (deleted) employees from the Employees table. Add a trigger to Employees table that inserts the corresponding information about the deleted records in Deleted_Employees.


CREATE TRIGGER tr_DeleteEmployees
  ON Employees
  AFTER DELETE
AS
  BEGIN
    INSERT INTO Deleted_Employees
      SELECT
        FirstName,
        LastName,
        MiddleName,
        JobTitle,
        DepartmentID,
        Salary
      FROM deleted
  END