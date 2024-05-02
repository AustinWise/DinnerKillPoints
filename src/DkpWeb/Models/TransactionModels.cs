using Austin.DkpLib;
using Sakura.AspNetCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DkpWeb.Models
{
    public class TopScoreEntry
    {
        public string Name { get; set; }
        [NotMapped]
        public Money Amount { get; set; }
        public int AmountPennies
        {
            get
            {
                return Amount.ToPennies();
            }
            set
            {
                this.Amount = new Money(value);
            }
        }
    }

    public class TransactionList
    {
        public TransactionList(int currentPage, int totalItemCount, IPagedList<Transaction> transactions)
        {
            this.CurrentPage = currentPage;
            this.TotalItems = totalItemCount;
            this.Transactions = transactions;
        }

        public const int PageSize = 50;

        public readonly int CurrentPage;
        public readonly int TotalItems;
        public readonly IPagedList<Transaction> Transactions;
    }

    public class NewTransactionModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        //[Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount (dollars)")]
        [Range(0.01, 10000)]
        public double Amount { get; set; }

        [Required]
        //[Display(Name = "Debtor")]
        public int Debtor { get; set; }

        [Required]
        //[Display(Name = "Creditor")]
        public int Creditor { get; set; }
    }
}
