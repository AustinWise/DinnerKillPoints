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
            var builder = WebApplication.CreateBuilder(args);
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services, builder.Environment);
            var app = builder.Build();
            startup.Configure(app, builder.Environment);

            var cfg = app.Configuration;

            if (cfg["migrate"] != null)
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await EnsureRole(roles, "Admin");
                await EnsureRole(roles, "DKP");
                return;
            }

            if (cfg["split"] != null)
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var peopleMap = db.Person.ToDictionary(p => p.Id);
                Person GetPerson(int id) => peopleMap[id];

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
                //var minh = GetPerson(14);
                var george = GetPerson(15);
                var justine = GetPerson(16);
                //var matt = GetPerson(17);
                var elaine = GetPerson(18);
                var roger = GetPerson(19);
                //var eric = GetPerson(20);
                var katherine = GetPerson(21);
                //var ryanSund = GetPerson(22);
                //var adrian = GetPerson(23);
                //var eddy = GetPerson(24);
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
                //var tiffany = GetPerson(37);
                var moriaki = GetPerson(38);
                var adam = GetPerson(39);
                var benKwong = GetPerson(40);
                var jasonBanich = GetPerson(41);
                //var changLiu = GetPerson(42);
                //var mike = GetPerson(43);
                //var spencer = GetPerson(44);
                //var jessie = GetPerson(45);
                var pat = GetPerson(46);
                var cam = GetPerson(47);
                //var zack = GetPerson(48);
                var mary = GetPerson(49);
                //var monica = GetPerson(50);
                //var ryanWeinstein = GetPerson(51);
                var michelle = GetPerson(52);
                var allison = GetPerson(53);
                var alexBruk = GetPerson(54);
                var dennis = GetPerson(55);
                //var salsa = GetPerson(57);
                var stephen = GetPerson(100);
                //var elisha = GetPerson(101);
                var maya = GetPerson(102);
                var charles = GetPerson(103);
                //var menglu = GetPerson(104);
                //var nate = GetPerson(105);
                //var ryanKwan = GetPerson(106);
                var will = GetPerson(107);
                var anthony = GetPerson(108);
                var helen = GetPerson(109);
                //var john = GetPerson(110);
                var kevin = GetPerson(111);


                WriteData(db, true, db.Person.Where(p => !p.IsDeleted).ToArray());
                WriteData(db, false, db.Person.ToArray());

                return;
            }

            if (cfg["sendmail"] != null)
            {
                using var scope = app.Services.CreateScope();
                var mail = scope.ServiceProvider.GetRequiredService<MailMerge>();
                await mail.Send(1);
                return;
            }

            string port = Environment.GetEnvironmentVariable("PORT");

            if (!string.IsNullOrEmpty(port))
            {
                await app.RunAsync("http://0.0.0.0:" + port);
            }
            else
            {
                await app.RunAsync();
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
