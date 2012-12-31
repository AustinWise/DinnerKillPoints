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
        private List<Tuple<Person, int>> mParty;

        public BillSpliter(string name, DateTime date, Person payer)
        {
            this.mName = name;
            this.mDate = date;
            this.mPayer = payer;
            mParty = new List<Tuple<Person, int>>();
        }

        public int this[Person person]
        {
            get
            {
                return mParty.Where(kvp => kvp.Item1.Equals(person)).Sum(p => p.Item2);
            }
            set
            {
                mParty.Add(new Tuple<Person, int>(person, value));
            }
        }
        public int Tax { get; set; }
        public int Tip { get; set; }
        public int SharedFood { get; set; }

        public void Save(DkpDataContext db)
        {

            var pool = mParty.Sum(p => p.Item2) + SharedFood;
            var bs = new BillSplit() { Name = mName };
            db.BillSplits.InsertOnSubmit(bs);

            foreach (var p in mParty)
            {
                double personSubtotal = SharedFood;
                personSubtotal /= mParty.Count;
                personSubtotal += p.Item2;
                var ratio = (double)personSubtotal / pool;
                Console.WriteLine(ratio);
                var total = personSubtotal + ratio * (Tax + Tip);
                Console.WriteLine(p.Item1.FirstName + ": " + total + " (" + Math.Round(total) + ")");

                if (p.Item1 == mPayer)
                    continue;

                var t = new Transaction()
                {
                    ID = Guid.NewGuid(),
                    Creditor = mPayer,
                    Debtor = p.Item1,
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
