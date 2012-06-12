using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DkpLib
{
    public class BillSpliter
    {
        private string mName;
        private DateTime mDate;
        private Person mPayer;

        public BillSpliter(string name, DateTime date, Person payer)
        {
            this.mName = name;
            this.mDate = date;
            this.mPayer = payer;
            this.Party = new Dictionary<Person, int>();
        }

        public Dictionary<Person, int> Party { get; private set; }
        public int Tax { get; set; }
        public int Tip { get; set; }
        public int SharedFood { get; set; }

        public void Save(DkpDataContext db)
        {

            var pool = Party.Sum(p => p.Value) + SharedFood;
            var bs = new BillSplit() { Name = mName };
            db.BillSplits.InsertOnSubmit(bs);

            foreach (var p in Party.Keys)
            {
                double personSubtotal = SharedFood;
                personSubtotal /= Party.Count;
                personSubtotal += Party[p];
                var ratio = (double)personSubtotal / pool;
                //Console.WriteLine(ratio);
                var total = personSubtotal + ratio * (Tax + Tip);
                //Console.WriteLine(p.FirstName + ": " + total + " (" + Math.Round(total) + ")");

                if (p == mPayer)
                    continue;

                var t = new Transaction()
                {
                    ID = Guid.NewGuid(),
                    Creditor = mPayer,
                    Debtor = p,
                    Amount = (int)Math.Round(total),
                    BillSplit = bs,
                    Description = mName,
                    Created = mDate,
                };
                db.Transactions.InsertOnSubmit(t);
            }

            db.SubmitChanges();
        }
    }
}
