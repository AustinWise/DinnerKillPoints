using Austin.DkpLib;
using DkpWeb.Data;
using DkpWeb.Models;
using DkpWeb.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DkpWeb
{
    public class MailMerge
    {
        readonly int[] PEOPLE_TO_EXCLUDE =
        {
            3, //wesley
            5, //dCastle
            11, //jeff
            12, //ryuho
            16, //justine
            34, //jimmy
            39, //adam
            42, //chang
        };

        readonly ApplicationDbContext mDb;
        readonly IEmailSender mEmail;
        readonly Dictionary<int, Person> mPersonMap;

        public MailMerge(ApplicationDbContext context, IEmailSender email)
        {
            mDb = context;
            mEmail = email;
            mPersonMap = context.Person.ToDictionary(p => p.Id);
        }

        public async Task Send(int creditorId)
        {
            bool actuallySend = false;
            Console.Write("Type 'true' to actually send emails: ");
            bool.TryParse(Console.ReadLine(), out actuallySend);

            var person = mDb.Person
                            .Where(p => p.Id == creditorId)
                            .Include(p => p.PaymentIdentity).ThenInclude(p => p.PaymentMeth)
                            .Single();

            var transactions = mDb.Transaction
                .Where(t => t.CreditorId != t.DebtorId
                    && (t.CreditorId == person.Id || t.DebtorId == person.Id)
                    && (!t.Creditor.IsDeleted && !t.Debtor.IsDeleted));
            foreach (var dontUse in PEOPLE_TO_EXCLUDE)
            {
                var p = dontUse;
                transactions = transactions.Where(t => t.DebtorId != p && t.CreditorId != p);
            }

            var netMoney = DebtGraph.TestAlgo(mDb, transactions, true, TextWriter.Null);

            var swGraph = new StringWriter();
            DebtGraph.WriteGraph(netMoney, swGraph);
            var bytes = DebtGraph.RenderGraphAsPng(swGraph.ToString());

            var debtors = DebtGraph.GreatestDebtor(netMoney);
            var myDebt = debtors.Where(d => d.Item1.Id == person.Id).SingleOrDefault();
            if (myDebt != null)
                debtors.Remove(myDebt);

            debtors = debtors.Where(tup => tup.Item2 > 900 && !string.IsNullOrEmpty(tup.Item1.Email)).ToList();

            int sentSoFar = 0;
            foreach (var tup in debtors)
            {
                var fields = ProcessOnePerson(person, tup.Item1, tup.Item2);

                if (actuallySend)
                    await mEmail.SendEmailAsync(tup.Item1.Email, "DKP Invoice", fields.BODY);
                Console.WriteLine(actuallySend ? "Sent {0,2}/{1,2} ({2} {3})" : "Would send: {2} {3}", ++sentSoFar, debtors.Count, tup.Item1.FirstName, tup.Item1.LastName);
            }

            Console.WriteLine();
            Console.WriteLine("Done! Press enter to exit.");
            Console.ReadLine();
        }

        class MyRecord
        {
            public string EmailAddress { get; set; }
            public string BODY { get; set; }
        }

        MyRecord ProcessOnePerson(Person creditor, Person debtor, int amount)
        {

            var q = from t in mDb.Transaction
                    where (t.CreditorId == debtor.Id && t.DebtorId == creditor.Id)
                       || (t.CreditorId == creditor.Id && t.DebtorId == debtor.Id)
                    orderby t.Created
                    select t;
            var allTrans = q.ToList();
            foreach (var t in allTrans)
            {
                t.SetPrettyDescription(mPersonMap);
            }

            var souceTrans = new List<Transaction>();
            Debt debt = null;
            while (allTrans.Count != 0)
            {
                debt = DebtGraph.TestAlgo(mDb, allTrans, false, TextWriter.Null).SingleOrDefault();
                if (debt == null)
                    break; //this indicates that there is no debt between the two people
                if (debt.Debtor.Id == creditor.Id)
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

            sb.Append("This is friendly, automated reminder that you currently owe a balence to me in DKP. ");
            sb.Append("Including ");
            sb.Append(souceTrans[0].Description);
            sb.Append(", our most recent time together, you owe me ");
            sb.AppendFormat("{0:c}.", amountInDollars);
            sb.AppendLine("<br/>");

            sb.Append("For more information about the transaction history see <a href=\"http://dkp.awise.us/MyDebt/DebtHistory?");
            sb.AppendFormat("debtorId={0}&creditorId={1}", debtor.Id, creditor.Id);
            sb.Append("\">this table</a>. Let me know if you have any questions or concerns.");
            sb.AppendLine("<br/>");

            sb.AppendLine("Here are some handy links to send payment:<ul>");
            foreach (var payId in creditor.PaymentIdentity)
            {
                if (!payId.PaymentMeth.HasPayLink)
                    continue;
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", payId.CreatePayLink(amount), payId.PaymentMeth.Name.Trim());
            }
            sb.Append("</ul>");
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
