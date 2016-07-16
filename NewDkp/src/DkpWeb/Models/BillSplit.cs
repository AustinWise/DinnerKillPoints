using System;
using System.Collections.Generic;

namespace DkpWeb.Models
{
    public partial class BillSplit
    {
        public BillSplit()
        {
            Transaction = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
