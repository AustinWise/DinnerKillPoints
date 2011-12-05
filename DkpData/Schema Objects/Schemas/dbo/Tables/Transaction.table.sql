CREATE TABLE [dbo].[Transaction] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [DebtorID]    INT              NOT NULL,
    [CreditorID]  INT              NOT NULL,
    [Amount]      INT              NOT NULL,
    [Created]     DATETIME2 (7)    NOT NULL,
    [Description] NVARCHAR (50)    NOT NULL,
    [BillID]      INT              NULL
);

