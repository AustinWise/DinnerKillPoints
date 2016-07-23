using System;
using System.Collections.Generic;
using System.Linq;

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


        string _PrettyName = null;
        public string PrettyName
        {
            get
            {
                return _PrettyName ?? Name;
            }
            set
            {
                _PrettyName = value;
            }
        }

        public void SetPrettyDescription(Dictionary<int, Person> personMap)
        {
            if (!Name.StartsWith(Models.Transaction.DebtTransferString))
                return;
            var amount = Transaction.Select(t => t.Amount).Distinct().Single();
            _PrettyName = Models.Transaction.CreatePrettyDescription(Name, amount, personMap);
        }
    }
}
