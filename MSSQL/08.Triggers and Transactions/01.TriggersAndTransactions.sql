--1.	Create Table Logs
--Create a table – Logs (LogId, AccountId, OldSum, NewSum). Add a trigger to the Accounts table that enters a new entry into the Logs table every time the sum on an account change. Submit only the query that creates the trigger.

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


SELECT * 
  FROM [Users]