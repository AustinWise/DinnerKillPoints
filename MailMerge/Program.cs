﻿using Austin.DkpLib;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace MailMerge
{
    class Program
    {
        const int PERSON_TO_MAIL_MERGE = 1;

        static readonly int[] PEOPLE_TO_EXCLUDE =
        {
            3, //wesley
            11, //jeff
            12, //ryuho
        };

        static DkpDataContext sDc;
        static Dictionary<int, Person> sPersonMap;

        static void Main(string[] args)
        {
            sDc = new DkpDataContext();
            sPersonMap = sDc.People.ToDictionary(p => p.ID, p => p);

            var person = sDc.People.Where(p => p.ID == PERSON_TO_MAIL_MERGE).Single();

            var transactions = sDc.Transactions
                .Where(t => t.CreditorID != t.DebtorID
                    && (t.CreditorID == person.ID || t.DebtorID == person.ID)
                    && (!t.Creditor.IsDeleted && !t.Debtor.IsDeleted));
            foreach (var dontUse in PEOPLE_TO_EXCLUDE)
            {
                var p = dontUse;
                transactions = transactions.Where(t => t.DebtorID != p && t.CreditorID != p);
            }

            var netMoney = DebtGraph.TestAlgo(sDc, transactions, true, TextWriter.Null);

            var swGraph = new StringWriter();
            DebtGraph.WriteGraph(netMoney, swGraph);
            var bytes = DebtGraph.RenderGraphAsPng(swGraph.ToString());

            var debtors = DebtGraph.GreatestDebtor(netMoney);
            var myDebt = debtors.Where(d => d.Item1.ID == person.ID).SingleOrDefault();
            if (myDebt != null)
                debtors.Remove(myDebt);

            debtors = debtors.Where(tup => tup.Item2 > 1000).ToList();

            using (var sw = new StreamWriter("test.csv"))
            {
                using (var csv = new CsvWriter(sw))
                {
                    csv.WriteHeader<MyRecord>();
                    foreach (var tup in debtors)
                    {
                        var fields = ProcessOnePerson(person, tup.Item1, tup.Item2);
                        csv.WriteRecord(fields);
                    }
                }

            }
        }

        class MyRecord
        {
            public string EmailAddress { get; set; }
            public string BODY { get; set; }
        }

        static MyRecord ProcessOnePerson(Person creditor, Person debtor, int amount)
        {

            var q = from t in sDc.Transactions
                    where (t.CreditorID == debtor.ID && t.DebtorID == creditor.ID)
                       || (t.CreditorID == creditor.ID && t.DebtorID == debtor.ID)
                    orderby t.Created
                    select t;
            var allTrans = q.ToList();
            foreach (var t in allTrans)
            {
                t.SetPrettyDescription(sPersonMap);
            }

            var souceTrans = new List<Transaction>();
            Debt debt = null;
            while (allTrans.Count != 0)
            {
                debt = DebtGraph.TestAlgo(sDc, allTrans, false, TextWriter.Null).SingleOrDefault();
                if (debt == null)
                    break; //this indicates that there is no debt between the two people
                if (debt.Debtor.ID == creditor.ID)
                    break; //this indicates that there is a debt owed in the opisite direction

                souceTrans.Add(allTrans[allTrans.Count - 1]);
                allTrans.RemoveAt(allTrans.Count - 1);
            }

            //TODO: grenerate a pretty table, maybe
            //It seems like a duplication of effort to try to create a subset of the website's debt table here.

            double amountInDollars = Math.Round(amount / 100d, 2);


            var sb = new StringBuilder();
            sb.AppendFormat("Hi {0},", debtor.FirstName);
            sb.AppendLine("<br/>");

            sb.Append("It is the end of the year so I am trying to balance my books.  ");
            sb.Append("Including ");
            sb.Append(souceTrans[0].Description);
            sb.Append(" you owe me ");
            sb.AppendFormat("{0:c}.", amountInDollars);
            sb.AppendLine("<br/>");

            sb.Append("For more information about the transaction history see <a href=\"http://dkp.awise.us/MyDebt/DebtHistory?");
            sb.AppendFormat("debtorId={0}&creditorId={1}", debtor.ID, creditor.ID);
            sb.Append("\">this table</a>. Let me know if you have any questions or concerns.");
            sb.AppendLine("<br/>");

            sb.Append("I accept Square Cash");
            sb.AppendFormat("(<a href=\"mailto:{0}?cc=cash@square.com&subject={1:c}\">click here to send</a>)",
                creditor.Email,
                amountInDollars);
            sb.Append(", Paypal, Venmo, cash, and check.");
            sb.AppendLine("<br/>");

            sb.Append("Thanks,");
            sb.AppendLine("<br/>");
            sb.Append(creditor.FirstName);

            var ret = new MyRecord();
            ret.EmailAddress = debtor.Email;
            ret.BODY = sb.ToString();
            return ret;
        }
    }
}