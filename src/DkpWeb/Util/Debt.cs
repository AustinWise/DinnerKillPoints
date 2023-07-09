using DkpWeb.Models;

namespace Austin.DkpLib
{
    public class Debt
    {
        public Money Amount { get; set; }
        public Person Creditor { get; set; }
        public Person Debtor { get; set; }

        public override string ToString()
        {
            return string.Format("{2}: {0} -> {1}", Debtor, Creditor, Amount);
        }

        public Debt Clone()
        {
            var ret = new Debt();
            ret.Creditor = this.Creditor.Clone();
            ret.Debtor = this.Debtor.Clone();
            ret.Amount = this.Amount;
            return ret;
        }
    }
}
