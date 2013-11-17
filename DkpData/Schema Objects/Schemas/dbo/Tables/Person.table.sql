CREATE TABLE [dbo].[Person] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (50) NOT NULL,
    [LastName]  NVARCHAR (50) NOT NULL,
    [IsDeleted] BIT           CONSTRAINT [DF_Person_IsDelete] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED ([ID] ASC)
);



