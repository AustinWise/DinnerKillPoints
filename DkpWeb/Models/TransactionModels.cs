using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Austin.DkpLib;

namespace DkpWeb.Models
{
    public class TransactionList
    {
        public TransactionList(int currentPage, int totalItemCount, IEnumerable<Transaction> transactions)
        {
            this.CurrentPage = currentPage;
            this.TotalItems = totalItemCount;
            this.Transactions = transactions;
        }

        public const int PageSize = 50;

        public readonly int CurrentPage;
        public readonly int TotalItems;
        public readonly IEnumerable<Transaction> Transactions;
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
        [Range(0.01, 100)]
        public double Amount { get; set; }

        [Required]
        //[Display(Name = "Debtor")]
        public int Debtor { get; set; }

        [Required]
        //[Display(Name = "Creditor")]
        public int Creditor { get; set; }
    }
}