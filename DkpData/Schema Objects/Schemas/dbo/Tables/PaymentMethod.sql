CREATE TABLE [dbo].[PaymentMethod]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NCHAR(50) NOT NULL, 
    [PayLinkFormat] NVARCHAR(MAX) NULL, 
    [RequestMoneyLinkFormat] NVARCHAR(MAX) NULL
)
