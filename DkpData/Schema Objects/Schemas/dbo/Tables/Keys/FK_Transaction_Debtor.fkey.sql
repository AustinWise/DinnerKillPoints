ALTER TABLE [dbo].[Transaction]
    ADD CONSTRAINT [FK_Transaction_Debtor] FOREIGN KEY ([DebtorID]) REFERENCES [dbo].[Person] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

