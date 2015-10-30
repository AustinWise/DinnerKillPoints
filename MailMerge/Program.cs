using Austin.DkpLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
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

        readonly static Encoding sPasswordEnc = Encoding.UTF8;
        readonly static DataProtectionScope sPasswordScope = DataProtectionScope.CurrentUser;

        static void Main(string[] args)
        {
            Properties.Settings settings = Properties.Settings.Default;
            string emailPassword;
            if (args.Length == 1 && args[0].ToLowerInvariant() == "--set-mail-password")
            {
                Console.Write("Enter email password: ");
                emailPassword = Console.ReadLine();
                byte[] encryptedPasword = ProtectedData.Protect(sPasswordEnc.GetBytes(emailPassword), null, sPasswordScope);
                string base64Password = Convert.ToBase64String(encryptedPasword);
                settings.EmailPassword = base64Password;
                settings.Save();
            }
            else
            {
                byte[] base64Password = Convert.FromBase64String(settings.EmailPassword);
                byte[] decryptedPassword = ProtectedData.Unprotect(base64Password, null, sPasswordScope);
                emailPassword = sPasswordEnc.GetString(decryptedPassword);
            }

            bool actuallySend = false;
            Console.Write("Type 'true' to actually send emails: ");
            bool.TryParse(Console.ReadLine(), out actuallySend);

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

            var from = new MailAddress(person.Email, person.ToString());
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new NetworkCredential(settings.EmailUser, emailPassword);
            client.EnableSsl = true;
            int sentSoFar = 0;
            int totalToSend = debtors.Where(d => !string.IsNullOrEmpty(d.Item1.Email)).Count();
            foreach (var tup in debtors.Where(d => !string.IsNullOrEmpty(d.Item1.Email)))
            {
                var fields = ProcessOnePerson(person, tup.Item1, tup.Item2);

                var msg = new MailMessage(from, new MailAddress(tup.Item1.Email, tup.Item1.ToString()));
                msg.Subject = "DKP Invoice";
                msg.Body = fields.BODY;
                msg.IsBodyHtml = true;

                if (actuallySend)
                    client.Send(msg);
                Console.WriteLine(actuallySend ? "Sent {0,2}/{1,2} ({2})" : "Would send: {2}", ++sentSoFar, totalToSend, tup.Item1.FirstName);
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

            sb.Append("This is friendly, automated reminder that you currently owe a balence to me in DKP. ");
            sb.Append("Including ");
            sb.Append(souceTrans[0].Description);
            sb.Append(", our most recent time together, you owe me ");
            sb.AppendFormat("{0:c}.", amountInDollars);
            sb.AppendLine("<br/>");

            sb.Append("For more information about the transaction history see <a href=\"http://dkp.awise.us/MyDebt/DebtHistory?");
            sb.AppendFormat("debtorId={0}&creditorId={1}", debtor.ID, creditor.ID);
            sb.Append("\">this table</a>. Let me know if you have any questions or concerns.");
            sb.AppendLine("<br/>");

            sb.AppendLine("Here are some handy links to send payment:<ul>");
            foreach (var payId in creditor.PaymentIdentities)
            {
                if (!payId.PaymentMethod.HasPayLink)
                    continue;
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", payId.CreatePayLink(amount), payId.PaymentMethod.Name.Trim());
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
