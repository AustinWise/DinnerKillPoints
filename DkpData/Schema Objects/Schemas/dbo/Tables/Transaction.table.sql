CREATE TABLE [dbo].[Transaction] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [DebtorID]    INT              NOT NULL,
    [CreditorID]  INT              NOT NULL,
    [Amount]      INT              NOT NULL,
    [Created]     DATETIME2 (7)    NOT NULL,
    [Description] NVARCHAR (50)    NOT NULL,
    [BillID]      INT              NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Transaction_BillSplit] FOREIGN KEY ([BillID]) REFERENCES [dbo].[BillSplit] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Transaction_Person_Creditor] FOREIGN KEY ([CreditorID]) REFERENCES [dbo].[Person] ([ID]),
    CONSTRAINT [FK_Transaction_Person_Debtor] FOREIGN KEY ([DebtorID]) REFERENCES [dbo].[Person] ([ID])
);



