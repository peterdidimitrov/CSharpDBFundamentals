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

