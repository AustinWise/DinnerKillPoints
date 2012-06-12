using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DkpLib
{
    public class Debt
    {
        public int Amount { get; set; }
        public Person Creditor { get; set; }
        public Person Debtor { get; set; }
    }
}
