using System.Diagnostics;

using MultiCreditorDebtList = System.Collections.Generic.List<(Austin.DkpLib.SplitPerson debtor, System.Collections.Generic.List<(Austin.DkpLib.SplitPerson creditor, double amount)> debts)>;

namespace Austin.DkpLib
{
    public class BillSplitter
    {
        class Debt
        {
            public SplitPerson Debtor { get; }
            public double Amount { get; }

            public Debt(SplitPerson debtor, double amount)
            {
                Debtor = debtor;
                Amount = amount;
            }
        }

        /// <summary>
        /// The maximum deviation from a rounded penny amount a sum is allowed to be.
        /// </summary>
        /// <remarks>
        /// The database stores all debts as an integer number of pennies. However during splitting a bill
        /// fractional penny debts can occure, such as when splitting an item worth an odd number of pennies between
        /// two people. Durring bill splits the algorithm uses this value as the maximum deviation a summ of all
        /// debts can be from an integer value, as a check some fractions of a penny have not been lost during rounding.
        /// </remarks>
        const double PENNY_THRESHOLD = 0.01;

        readonly string mName;
        readonly DateTime mDate;
        readonly SplitPerson[] mPayer;
        readonly List<Debt> mParty;
        readonly SortedSet<SplitPerson> mFreeLoaders;
        readonly SortedSet<SplitPerson> mFremontBirthday;

        public BillSplitter(string name, DateTime date, params SplitPerson[] payer)
        {
            if (payer == null)
                throw new ArgumentNullException(nameof(payer));
            if (payer.Length < 1)
                throw new ArgumentOutOfRangeException(nameof(payer), "Need at least one payer.");
            if (date.Year < 2010)
                throw new ArgumentOutOfRangeException(nameof(date), "The date is prior to existence of DKP, that seems unlikly.");

            mName = name;
            mDate = date.ToUniversalTime();
            mPayer = (SplitPerson[])payer.Clone();
            mParty = new List<Debt>();
            mFreeLoaders = new SortedSet<SplitPerson>();
            mFremontBirthday = new SortedSet<SplitPerson>();
        }

        public double this[SplitPerson person]
        {
            get
            {
                var trans = mParty.Where(kvp => kvp.Debtor.Equals(person)).ToList();
                if (trans.Count == 0)
                    throw new ArgumentException($"'{person}' is has no transactions in this billsplit.");
                return trans.Sum(p => p.Amount);
            }
            set
            {
                mParty.Add(new Debt(person, value));
            }
        }

        public int Tax { get; set; }
        public int Tip { get; set; }
        public int SharedFood { get; set; }
        public int Discount { get; set; }

        public int SubTotal
        {
            get
            {
                return Convert.ToInt32(mParty.Sum(p => p.Amount)) + SharedFood;
            }
        }

        public void AddFreeLoader(SplitPerson p)
        {
            bool added = mFreeLoaders.Add(p);
            Debug.Assert(added);
        }

        public void AddFremontBirthday(SplitPerson p)
        {
            bool added = mFremontBirthday.Add(p);
            Debug.Assert(added);
        }

        public IEnumerable<Transaction> ToTransactions()
        {
            return ToTransactions(TextWriter.Null);
        }

        /// <summary>
        /// Generate transactions without persisting to a database.
        /// </summary>
        public IEnumerable<Transaction> ToTransactions(TextWriter log)
        {
            var debts = SplitBill(log);

            foreach (var (debtor, creditor, pennies) in debts)
            {
                yield return new Transaction()
                {
                    Id = Guid.NewGuid(),
                    CreditorId = creditor.Id,
                    DebtorId = debtor.Id,
                    Amount = pennies,
                    Created = mDate,
                };
            }
        }

        private void ValidateBill()
        {
            if (mParty.Count == 0)
                throw new Exception("Must have one or more people in that party.");
            foreach (var kvp in mParty)
            {
                if (kvp.Amount < 0)
                    throw new NotSupportedException(kvp.Debtor.ToString() + " spent a negitive amount of money.");
            }
            if (Tax < 0 || Tip < 0 || SharedFood < 0 || Discount < 0)
                throw new NotSupportedException("Negative bill attribute.");
        }

        public List<(SplitPerson debtor, SplitPerson creditor, Money pennies)> SplitBill(TextWriter log)
        {
            ValidateBill();

            var pool = mParty.Sum(p => p.Amount) + SharedFood;
            var totalBillValue = pool + Tip + Tax - Discount;

            if (totalBillValue <= 0)
                throw new Exception("Zero or negative total bill value.");
            if (Math.Abs(totalBillValue - Math.Round(totalBillValue)) > PENNY_THRESHOLD)
                throw new Exception("Non-int number of pennies.");

            log.WriteLine($"name: {this.mName}");
            log.WriteLine($"date: {this.mDate}");
            log.WriteLine($"payer(s): {string.Join(", ", mPayer.Select(p => p.FullName))}");
            log.WriteLine($"pool: {pool / 100:c}");
            log.WriteLine($"totalBillValue: {totalBillValue / 100:c}");
            log.WriteLine();

            var amountSpent = new List<Debt>();

            log.WriteLine("First add up each person's share of the bill, splitting shared food items equally:");
            foreach (var p in mParty)
            {
                log.WriteLine($"\t{p.Debtor.FullName}:");

                double personSubtotal = SharedFood;
                personSubtotal /= mParty.Count;
                log.WriteLine($"\t\tpersonal food: {p.Amount / 100:c}");
                log.WriteLine($"\t\tshared food share: {personSubtotal / 100:c}");
                personSubtotal += p.Amount;
                var ratio = personSubtotal / pool;
                log.WriteLine($"\t\ttax, tip, and discounts share: %{ratio * 100}");
                var taxTipShare = ratio * (Tax + Tip - Discount);
                log.WriteLine($"\t\ttax, tip, and discounts share: {taxTipShare / 100:c}");
                var total = personSubtotal + taxTipShare;

                amountSpent.Add(new Debt(p.Debtor, total));
                log.WriteLine($"\t\ttotal raw share {total / 100:c}");
            }
            checkTotal(totalBillValue, amountSpent.Select(tup => tup.Amount).Sum());
            log.WriteLine();

            //Apply Fremont-style birthday redistrobution
            var birthdayPeople = amountSpent.Where(p => mFremontBirthday.Contains(p.Debtor)).ToList();
            if (birthdayPeople.Count != 0)
            {
                log.WriteLine("Applying Fremont-style birthday logic.");

                //first zero the birthpeople's debts
                amountSpent = amountSpent
                    .Select(tup => new Debt(tup.Debtor, mFremontBirthday.Contains(tup.Debtor) ? 0 : tup.Amount))
                    .ToList();

                //Then redistribute
                foreach (var birthdayPerson in birthdayPeople)
                {
                    var amount = birthdayPerson.Amount / (amountSpent.Count - 1);
                    log.WriteLine($"\t{birthdayPerson.Debtor}'s birthday happiness cost each other person {amount / 100:c}.");
                    amountSpent = amountSpent
                        .Select(tup => new Debt(tup.Debtor, tup.Amount + (tup.Debtor == birthdayPerson.Debtor ? 0 : amount)))
                        .ToList();
                }

                checkTotal(totalBillValue, amountSpent.Sum(p => p.Amount));
                log.WriteLine();
            }

            //Take each freeloader and evenly split their meal across all the non-freeloaders
            var freeloadersFound = amountSpent.Where(p => mFreeLoaders.Contains(p.Debtor)).ToList();
            var nonFreeLoaderCount = amountSpent.Count - freeloadersFound.Count;
            if (freeloadersFound.Count != 0)
            {
                var freeloaderSum = freeloadersFound.Select(p => p.Amount).Sum();
                log.WriteLine($"{freeloadersFound.Count} freeloaders found, owing a total of {freeloaderSum / 100:c}:");
                foreach (var freeloader in freeloadersFound)
                {
                    log.WriteLine($"\t{freeloader.Debtor.FullName}: {freeloader.Amount / 100:c}");
                }
                var extraPerPerson = freeloaderSum / nonFreeLoaderCount;
                log.WriteLine($"adding {extraPerPerson / 100:c} to each non-freeloader's debts");
                amountSpent = amountSpent
                    .Where(p => !mFreeLoaders.Contains(p.Debtor))
                    .Select(p => new Debt(p.Debtor, p.Amount + freeloaderSum / nonFreeLoaderCount))
                    .ToList();
                foreach (var p in freeloadersFound)
                {
                    amountSpent.Add(new Debt(p.Debtor, 0d));
                }

                checkTotal(totalBillValue, amountSpent.Sum(p => p.Amount));
                log.WriteLine();
            }

            //Evenly split each person's debt to each payer.
            var debtsToPayers = SplitDebtsBetweenPayers(amountSpent);
            checkTotal(totalBillValue, debtsToPayers.SelectMany(p => p.debts).Sum(p => p.amount));

            //Take all the fractional pennies and distrubte them to each debtor, round robin to each payer.
            var pennySplits = SplitPennies(debtsToPayers);
            checkTotal(totalBillValue, pennySplits.Sum(p => p.pennies.ToPennies()));

            foreach (var p in pennySplits)
            {
                if (p.pennies < Money.Zero)
                    throw new Exception("Negative debt.");
            }

            return pennySplits;
        }

        static void checkTotal(double initalSum, double currentSum)
        {
            if (Math.Abs(currentSum - Math.Round(currentSum)) > PENNY_THRESHOLD)
                throw new Exception("Non-int number of pennies.");
            if (Math.Abs(initalSum - currentSum) > 0.1)
                throw new Exception("Something terrible has happened.");
        }

        MultiCreditorDebtList SplitDebtsBetweenPayers(List<Debt> amountSpent)
        {
            var ret = new MultiCreditorDebtList();
            foreach (var p in amountSpent)
            {
                var creditors = new List<(SplitPerson creditor, double amount)>();
                foreach (var payer in mPayer)
                {
                    creditors.Add((payer, p.Amount / mPayer.Length));
                }
                ret.Add((p.Debtor, creditors));
            }
            return ret;
        }

        /// <summary>
        /// Round splits to the nearest penny.
        /// </summary>
        /// <param name="amountSpent"></param>
        /// <returns>Item1 owes Item2 Item3 pennies</returns>
        /// <remarks>
        /// Over the course of splitting a bill people can end up owing a fractional number of pennies.
        /// Thie method rounds peoples debts to the nearest penny, while preserving the invarient that
        /// the total amount owed in this group of debts does not change.
        /// </remarks>
        List<(SplitPerson debtor, SplitPerson creditor, Money pennies)> SplitPennies(MultiCreditorDebtList amountSpent)
        {
            var pennies = amountSpent
                .Select(tup => new
                {
                    Debtor = tup.debtor,
                    Debts = tup.debts.Select(debt => (creditor: debt.creditor, amount: (int)Math.Floor(debt.amount))).ToList(),
                    PennyFraction = tup.debts.Select(debt => debt.amount - Math.Floor(debt.amount)).Sum()
                })
                .OrderByDescending(p => p.PennyFraction)
                .ToList();
            var tempDoublePennyCount = pennies.Sum(p => p.PennyFraction);

            if (Math.Abs(tempDoublePennyCount - Math.Round(tempDoublePennyCount)) > PENNY_THRESHOLD)
                throw new Exception("Non-int number of pennies.");

            var totalPennies = (int)Math.Round(tempDoublePennyCount);

            if (totalPennies < 0)
                throw new Exception("Negitive number of pennies.");


            int payerNdx = 0;
            while (totalPennies != 0)
            {
                foreach (var d in pennies)
                {
                    if (totalPennies == 0)
                        break;
                    if (mFreeLoaders.Contains(d.Debtor))
                        continue;
                    var ndx = payerNdx++ % d.Debts.Count;
                    var (creditor, amount) = d.Debts[ndx];
                    d.Debts[ndx] = (creditor, amount + 1);
                    totalPennies--;
                }
            }

            return pennies.SelectMany(p => p.Debts.Select(d => (p.Debtor, d.creditor, new Money(d.amount)))).ToList();
        }
    }
}
