using System;
using System.Collections.Generic;

namespace DkpWeb.Models
{
    public partial class Person
    {
        public Person()
        {
            PaymentIdentity = new HashSet<PaymentIdentity>();
            TransactionCreditor = new HashSet<Transaction>();
            TransactionDebtor = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        public string Email { get; set; }

        public virtual ICollection<PaymentIdentity> PaymentIdentity { get; set; }
        public virtual ICollection<Transaction> TransactionCreditor { get; set; }
        public virtual ICollection<Transaction> TransactionDebtor { get; set; }
    }
}
