using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models.MyDebtViewModels
{
    public class DebtLedger
    {
        [Required]
        public List<DebtLedgerEntry> Entries { get; set; }

        [Required]
        public Person Debtor { get; set; }

        [Required]
        public Person Creditor { get; set; }

        [Required]
        public int AmountCents { get; set; }

        [Required]
        public string AmountDollars { get; set; }
    }
}
