using Austin.DkpLib;
using DkpWeb.Data;
using DkpWeb.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DkpWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var allSplitPeople = new List<SplitPerson>();
            int idCounter = 0;
            var austin = new SplitPerson()
            {
                Id = ++idCounter,
                FullName = "Austin",
            };
            allSplitPeople.Add(austin);
            var wesley = new SplitPerson()
            {
                Id = ++idCounter,
                FullName = "Wesley",
            };
            allSplitPeople.Add(wesley);
            var zoe = new SplitPerson()
            {
                Id = ++idCounter,
                FullName = "Zoe",
            };
            allSplitPeople.Add(zoe);
            var carena = new SplitPerson()
            {
                Id = ++idCounter,
                FullName = "Carena",
            };
            allSplitPeople.Add(carena);


            var trans = new List<SplitTransaction>();

            {
                var bs = new BillSplitter("Mongolie Grill", new DateTime(2026, 3, 1, 12 + 7, 28, 0), zoe);
                bs.Tip = 3076;
                bs.Tax = 799 + 320;
                bs[zoe] = 3392 + 900; // 0.53 kg + lychee lemonade
                bs[wesley] = 1600 + 4384; // steamworks + 0.685 kg
                bs[austin] = 1600 + 4096; // steamworks + 0.640 kg
                trans.AddRange(bs.ToTransactions(Console.Out));
            }

            {
                var bs = new BillSplitter("The Keg", new DateTime(2026, 3, 2, 12 + 7, 7, 0), austin);
                bs.Tax = 625 + 77 + 1233;
                bs.Tip = 5317;
                bs[austin] = 4200 + 1000 + 2100; // 6oz sirloin + molsen + syrah
                bs[zoe] = 5500 + 1400 + 1100; // 6 oz sirloin / classic + truffle + mock micha colda
                bs[wesley] = 6200 + 1050 + 2100; // 10 oz prime rib + pale ale + syrah
                trans.AddRange(bs.ToTransactions(Console.Out));
            }

            {
                var bs = new BillSplitter("Whistler Grocery Store", new DateTime(2026, 3, 2, 19, 28, 0), austin);
                bs.Tax = 74;
                bs.SharedFood = 9716;
                bs[austin] = 0;
                bs[wesley] = 0;
                bs[zoe] = 0;
                bs[carena] = 0;
                trans.AddRange(bs.ToTransactions(Console.Out));
            }

            {
                var bs = new BillSplitter("Longhorn Saloon", new DateTime(2026, 3, 3, 12, 29, 0), zoe);
                bs.SharedFood = 3500 + 500 * 4; // fucking nachos
                bs.Tax = 32 + 430 + 777;
                bs.Tip = 3357;
                bs[wesley] = 1100 + 1100; // "cervesa" and budweiser
                bs[austin] = 1000 + 2700 + 1100; // apres lager and "alpine" chicken burger and budweiser
                bs[zoe] = 450 + 2400 + 200; // diest coke and wings
                trans.AddRange(bs.ToTransactions(Console.Out));
            }

            {
                var bs = new BillSplitter("The Old Spaghetti Factory", new DateTime(2026, 3, 4, 12 + 4, 45, 0), austin);
                bs.SharedFood = 1095 - 195; // shrimp, minus happy hour
                bs.Tax = 701 + 432;
                bs.Tip = 3032;
                bs[wesley] = 795 - 195 + 2050 + 795; // linquine
                bs[austin] = (1390 - 190) / 2 + 1990 + 1025 + 295;
                bs[carena] = (1390 - 190) / 2 + 2175;
                bs[zoe] = 825 - 125 + 2295; // fettuccine alfredo
                trans.AddRange(bs.ToTransactions(Console.Out));
            }

            {
                var bs = new BillSplitter("21 Steps", new DateTime(2026, 3, 5, 12 + 6, 32, 0), austin);
                bs.SharedFood = 1700 + 1400; // fried tofu + cheesecake
                bs.Tax = 1285 + 890;
                bs.Tip = 5575;
                bs[austin] = 1600 + 1700 + 800 + 2100; // corpse reviver #2 + division bell + fernet + salad
                bs[wesley] = 1700 + 3000; // 2x rotator (stout) + pork pappardelle
                bs[zoe] = 1500 + 3700; // riesling + lingcod
                bs[carena] = 1600 + 4900; // new york steak
                trans.AddRange(bs.ToTransactions(Console.Out));
            }

            {
                var bs = new BillSplitter("Bar Oso", new DateTime(2026, 3, 6, 12 + 5, 41, 0), austin);
                bs.Tax = 1475 + 1370;
                bs.Tip = 6469;
                bs.SharedFood = 1400 + 2800 + 1200 + 1900 + 1900 + 4800 + 1800;
                bs[austin] = 2000 + 1900;
                bs[wesley] = 2000 + 1900;
                bs[zoe] = 2000;
                bs[carena] = 2000 + 1900;
                trans.AddRange(bs.ToTransactions(Console.Out));
            }

            var peopleMap = allSplitPeople.ToDictionary(p => p.Id, p => new Person()
            {
                Id = p.Id,
                FirstName = p.FullName,
            });
            var allTrans = trans.Select(t => new Transaction()
            {
                Id = t.Id,
                Amount = t.Amount,
                Creditor = peopleMap[t.CreditorId],
                CreditorId = t.CreditorId,
                Debtor = peopleMap[t.DebtorId],
                DebtorId = t.DebtorId,
            }).ToArray();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Calculating Final Debts");
            var debts = DebtGraph.CalculateDebts(allTrans, true, Console.Out);
            Console.WriteLine();
            Console.WriteLine("FINAL TOTAL:");
            Console.WriteLine();
            foreach (var debt in debts)
            {
                Console.WriteLine(debt);
            }
        }

        private static void WriteData(ApplicationDbContext db, bool removeCycles, Person[] people)
        {
            const string outDir = @"E:\AustinWise\OneDrive\DKP";

            List<Debt> netMoney = null;
            using (Stream fs = removeCycles ? new FileStream(Path.Combine(outDir, "Info.txt"), FileMode.Create, FileAccess.Write) : Stream.Null)
            using (var infoOutput = new StreamWriter(fs))
            {
                netMoney = DebtGraph.CalculateDebts(db, people, removeCycles, infoOutput);
            }
            Console.WriteLine("{0}", netMoney.Sum(m => m.Amount));

            string gvPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".gv");
            try
            {
                using (var fs = new FileStream(gvPath, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    DebtGraph.WriteGraphviz(netMoney, sw);
                }

                string outFile = Path.Combine(outDir, removeCycles ? "current.png" : "nocycles.png");
                DebtGraph.RenderGraphAsPng(gvPath, outFile);
                if (removeCycles)
                    File.Copy(outFile, Path.Combine(outDir, DateTime.Now.ToString("yyyy-MM-dd") + ".png"), overwrite: true);
            }
            finally
            {
                File.Delete(gvPath);
            }
        }

        private static void DebtTransfer(ApplicationDbContext db, Person debtor, Person oldCreditor, Person newCreditor)
        {
            DebtTransfer(db, debtor, oldCreditor, newCreditor, DateTime.Now);
        }

        private static void DebtTransfer(ApplicationDbContext db, Person debtor, Person oldCreditor, Person newCreditor, DateTime when)
        {
            var netMoney = DebtGraph.CalculateDebts(db, new[] { debtor, oldCreditor }, false, null);
            if (netMoney.Count != 1)
                throw new Exception("No debt to transfer.");

            var theDebt = netMoney[0];

            if (theDebt.Debtor.Id != debtor.Id || theDebt.Creditor.Id != oldCreditor.Id)
                throw new Exception("Debt does not go in the expected direction.");

            var msg = Transaction.CreateDebtTransferString(debtor, oldCreditor, newCreditor);

            var bs = new BillSplit
            {
                Name = msg
            };
            db.BillSplit.Add(bs);

            var cancelTrans = new Transaction()
            {
                Id = Guid.NewGuid(),
                DebtorId = oldCreditor.Id, //owes money
                CreditorId = debtor.Id, //owed money
                Amount = theDebt.Amount,
                Bill = bs,
                Description = msg,
                Created = when
            };
            db.Transaction.Add(cancelTrans);

            var makeCreditorWholeTransaction = new Transaction()
            {
                Id = Guid.NewGuid(),
                DebtorId = newCreditor.Id, //owes money
                CreditorId = oldCreditor.Id, //owed money
                Amount = theDebt.Amount,
                Bill = bs,
                Description = msg,
                Created = when
            };
            db.Transaction.Add(makeCreditorWholeTransaction);

            var makeDebtorOweNewPartyTrans = new Transaction()
            {
                Id = Guid.NewGuid(),
                DebtorId = debtor.Id, //owes money
                CreditorId = newCreditor.Id, //owed money
                Amount = theDebt.Amount,
                Bill = bs,
                Description = msg,
                Created = when
            };
            db.Transaction.Add(makeDebtorOweNewPartyTrans);

            db.SaveChanges();
        }

        static async Task EnsureRole(RoleManager<IdentityRole> roles, string name)
        {
            var role = await roles.FindByNameAsync(name);
            if (role == null)
                await roles.CreateAsync(new IdentityRole(name));
        }
    }
}
