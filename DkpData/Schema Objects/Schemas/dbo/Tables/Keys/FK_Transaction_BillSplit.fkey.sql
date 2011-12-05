ALTER TABLE [dbo].[Transaction]
    ADD CONSTRAINT [FK_Transaction_BillSplit] FOREIGN KEY ([BillID]) REFERENCES [dbo].[BillSplit] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

