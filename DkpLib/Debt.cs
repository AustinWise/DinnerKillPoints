using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DkpLib
{
    public class Debt : ICloneable
    {
        public int Amount { get; set; }
        public Person Creditor { get; set; }
        public Person Debtor { get; set; }

        public override string ToString()
        {
            return string.Format("{2}: {0} -> {1}", Debtor, Creditor, Amount);
        }

        public object Clone()
        {
            var ret = new Debt();
            ret.Creditor = (Person)this.Creditor.Clone();
            ret.Debtor = (Person)this.Debtor.Clone();
            ret.Amount = this.Amount;
            return ret;
        }
    }
}
