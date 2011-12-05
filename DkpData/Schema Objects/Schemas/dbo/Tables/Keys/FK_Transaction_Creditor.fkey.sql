ALTER TABLE [dbo].[Transaction]
    ADD CONSTRAINT [FK_Transaction_Creditor] FOREIGN KEY ([CreditorID]) REFERENCES [dbo].[Person] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

