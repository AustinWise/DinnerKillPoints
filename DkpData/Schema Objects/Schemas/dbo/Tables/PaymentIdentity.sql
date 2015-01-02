CREATE TABLE [dbo].[PaymentIdentity]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PersonID] INT NOT NULL, 
    [PaymentMethID] INT NOT NULL, 
    [UserName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_PaymentIdentity_Person] FOREIGN KEY ([PersonID]) REFERENCES [Person]([ID]), 
    CONSTRAINT [FK_PaymentIdentity_PaymentMethod] FOREIGN KEY ([PaymentMethID]) REFERENCES [PaymentMethod]([ID])
)
