using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Austin.DkpLib;

namespace DkpWeb.Models
{
    public class MyDebtModel
    {
        [Required]
        public Person Person { get; set; }

        [Required]
        public string LogOutput { get; set; }

        [Required]
        public string ImageBase64 { get; set; }

        [Required]
        public List<Tuple<Person, int>> Creditors { get; set; }

        [Required]
        public int OverallDebt { get; set; }
    }

    public class DebtLedgerEntry
    {
        public DebtLedgerEntry(Transaction t, int amount, int runningTotal)
        {
            this.TransactionId = t.ID;
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

    public class DebtLedger
    {
        [Required]
        public List<DebtLedgerEntry> Entries { get; set; }

        [Required]
        public Person Debtor { get; set; }

        [Required]
        public Person Creditor { get; set; }
    }
}