using Austin.DkpLib;
using System;
using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models.MyDebtViewModels
{
    public class DebtLedgerEntry
    {
        public DebtLedgerEntry(Transaction t, Money amount, Money runningTotal)
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
        public Money Amount { get; set; }

        [Required]
        public Money RunningTotal { get; set; }
    }
}
