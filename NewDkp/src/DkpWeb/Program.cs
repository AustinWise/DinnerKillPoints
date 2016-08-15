using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using DkpWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DkpWeb.Data;
using Austin.DkpLib;

namespace DkpWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            var roles = host.Services.GetService<RoleManager<IdentityRole>>();
            EnsureRole(roles, "Admin").Wait();
            EnsureRole(roles, "DKP").Wait();

            if (args.Any(a => a == "--split"))
            {
                var db = host.Services.GetService<ApplicationDbContext>();
                Func<int, Person> GetPerson = id => db.Person.Where(p => p.Id == id).Single();

                var austin = GetPerson(1);
                var caspar = GetPerson(2);
                var wesley = GetPerson(3);
                //var maria = GetPerson(4);
                var david = GetPerson(5);
                //var seanMc = GetPerson(6);
                //var andrea = GetPerson(7);
                //var meredith = GetPerson(8);
                var seanChen = GetPerson(9);
                var arata = GetPerson(10);
                var jeff = GetPerson(11);
                var ryuho = GetPerson(12);
                //var laura = GetPerson(13);
                var minh = GetPerson(14);
                var george = GetPerson(15);
                var justine = GetPerson(16);
                var matt = GetPerson(17);
                var elaine = GetPerson(18);
                var roger = GetPerson(19);
                var eric = GetPerson(20);
                var katherine = GetPerson(21);
                //var ryanSund = GetPerson(22);
                var adrian = GetPerson(23);
                var ed = GetPerson(24);
                var randy = GetPerson(25);
                //var becky = GetPerson(26);
                var andrew = GetPerson(29);
                var justinShih = GetPerson(30);
                //var davidFang = GetPerson(31);
                var derek = GetPerson(32);
                //var elaineJeu = GetPerson(33);
                var jimmy = GetPerson(34);
                var alexFong = GetPerson(35);
                var victor = GetPerson(36);
                var tiffany = GetPerson(37);
                var moriaki = GetPerson(38);
                var adam = GetPerson(39);
                var benKwong = GetPerson(40);
                var jasonBanich = GetPerson(41);
                var changLiu = GetPerson(42);
                var mike = GetPerson(43);
                //var spencer = GetPerson(44);
                //var jessie = GetPerson(45);
                var pat = GetPerson(46);
                var cam = GetPerson(47);
                var zack = GetPerson(48);
                var mary = GetPerson(49);
                var monica = GetPerson(50);
                var ryanWeinstein = GetPerson(51);
                var michelle = GetPerson(52);
                var allison = GetPerson(53);
                var alexBruk = GetPerson(54);
                var dennis = GetPerson(55);
                var salsa = GetPerson(57);

                //var bs = new BillSpliter("Drinks", new DateTime(2016, 7, 22, 12 + 10, 0, 0), austin);
                //bs.SharedFood = 2640;
                //bs[austin] = 0;
                //bs[justinShih] = 300;
                //bs.Save(db);


                WriteData(db, true, db.Person.Where(p => !p.IsDeleted).ToArray());
                WriteData(db, false, db.Person.ToArray());

                return;
            }

            host.Run();
        }

        private static void WriteData(ApplicationDbContext db, bool removeCycles, Person[] people)
        {
            const string outDir = @"D:\AustinWise\Dropbox\DKP";

            List<Debt> netMoney = null;
            TextWriter infoOutput = removeCycles ? new StreamWriter(File.OpenWrite(Path.Combine(outDir, "Info.txt"))) : null;
            netMoney = DebtGraph.TestAlgo(db, people, removeCycles, infoOutput ?? Console.Out);
            if (infoOutput != null)
                infoOutput.Dispose();
            Console.WriteLine("{0:c}", netMoney.Sum(m => m.Amount) / 100d);

            const string gvPath = @"c:\temp\graph\test.gv";
            using (var sw = new StreamWriter(File.OpenWrite(gvPath)))
            {
                DebtGraph.WriteGraph(netMoney, sw);
            }

            DebtGraph.RenderGraphAsPng(gvPath, Path.Combine(outDir, removeCycles ? "current.png" : "nocycles.png"));
            if (removeCycles)
                DebtGraph.RenderGraphAsPng(gvPath, Path.Combine(outDir, DateTime.Now.ToString("yyyy-MM-dd") + ".png"));
        }

        private static void DebtTransfer(ApplicationDbContext db, Person debtor, Person oldCreditor, Person newCreditor)
        {
            DebtTransfer(db, debtor, oldCreditor, newCreditor, DateTime.Now);
        }

        private static void DebtTransfer(ApplicationDbContext db, Person debtor, Person oldCreditor, Person newCreditor, DateTime when)
        {
            var netMoney = DebtGraph.TestAlgo(db, new[] { debtor, oldCreditor }, false, null);
            if (netMoney.Count != 1)
                throw new Exception("No debt to transfer.");

            var theDebt = netMoney[0];

            if (theDebt.Debtor.Id != debtor.Id || theDebt.Creditor.Id != oldCreditor.Id)
                throw new Exception("Debt does not go in the expected direction.");

            var msg = Transaction.CreateDebtTransferString(debtor, oldCreditor, newCreditor);

            var bs = new BillSplit();
            bs.Name = msg;
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
