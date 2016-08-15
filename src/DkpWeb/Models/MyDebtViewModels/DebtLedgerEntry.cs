using System;
using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models.MyDebtViewModels
{
    public class DebtLedgerEntry
    {
        public DebtLedgerEntry(Transaction t, int amount, int runningTotal)
        {
            this.TransactionId = t.Id;
            this.Description = t.PrettyDescription;
            this.Created = t.Created;
            this.Amount = amount;
            this.RunningTotal = runningTotal;
        }

        [Required]
        public Guid TransactionId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public int RunningTotal { get; set; }
    }
}
