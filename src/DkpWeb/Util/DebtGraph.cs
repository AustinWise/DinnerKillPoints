using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using DkpWeb.Models;
using DkpWeb.Data;
using Microsoft.AspNetCore.Html;

namespace Austin.DkpLib
{
    public static class DebtGraph
    {
        static List<Tuple<int, int>> sMutuallyAssuredUnpayment = new List<Tuple<int, int>>();
        private static void AddUnPayment(int p1, int p2)
        {
            sMutuallyAssuredUnpayment.Add(new Tuple<int, int>(p1, p2));
            sMutuallyAssuredUnpayment.Add(new Tuple<int, int>(p2, p1));
        }

        public static List<Debt> CalculateDebts(IEnumerable<Transaction> rawTrans, bool RemoveCycles, TextWriter logger)
        {
            var people = rawTrans.SelectMany(t => new[] { t.Creditor, t.Debtor }).Distinct().ToArray();
            return CalculateDebts(rawTrans, people, RemoveCycles, logger);
        }

        public static List<Debt> CalculateDebts(ApplicationDbContext db, Person[] people, bool RemoveCycles, TextWriter logger)
        {
            return CalculateDebts(db.Transaction.Where(t => t.CreditorId != t.DebtorId), people, RemoveCycles, logger);
        }

        public static List<Debt> CalculateDebts(IEnumerable<Transaction> trans, Person[] people, bool RemoveCycles, TextWriter logger)
        {
            var netMoney = new List<Debt>();
            var peopleMap = people.ToDictionary(p => p.Id);

            logger = logger ?? TextWriter.Null;

            //sum all debts from one person to another
            var summedDebts = new Dictionary<Tuple<Person, Person>, int>();
            foreach (var t in trans)
            {
                var tup = new Tuple<Person, Person>(peopleMap[t.DebtorId], peopleMap[t.CreditorId]);
                int net = 0;
                if (summedDebts.ContainsKey(tup))
                    net = summedDebts[tup];
                net += t.Amount;
                summedDebts[tup] = net;
            }

            if (RemoveCycles)
            {
                //remove mutually assured unpayment
                foreach (var mut in sMutuallyAssuredUnpayment)
                {
                    summedDebts.Remove(new Tuple<Person, Person>(peopleMap[mut.Item1], peopleMap[mut.Item2]));
                }
            }

            //net up all the debts
            //what's this, O(2n) ?
            for (int i = 0; i < people.Length - 1; i++)
            {
                var p1 = people[i];
                for (int j = i + 1; j < people.Length; j++)
                {
                    var p2 = people[j];
                    var net = 0;
                    int temp = 0;
                    if (summedDebts.TryGetValue(new Tuple<Person, Person>(p1, p2), out temp))
                    {
                        net = temp;
                    }
                    if (summedDebts.TryGetValue(new Tuple<Person, Person>(p2, p1), out temp))
                    {
                        net -= temp;
                    }
                    if (net > 0)
                        netMoney.Add(new Debt() { Debtor = p1, Creditor = p2, Amount = net });
                    else if (net < 0)
                        netMoney.Add(new Debt() { Debtor = p2, Creditor = p1, Amount = -net });
                }
            }

            if (RemoveCycles)
            {
                logger.WriteLine("Cycles");
                bool foundCycle;
                do
                {
                    foundCycle = false;
                    foreach (var cycle in FindCycles(people, netMoney))
                    {
                        if (cycle.Count != 1)
                            foundCycle = true;
                        logger.WriteLine("->");
                        foreach (var p in cycle)
                        {
                            logger.WriteLine("\t{0}", p.FirstName);
                        }

                        var cycleTrans = new List<Debt>();
                        for (int i = 0; i < cycle.Count; i++)
                        {
                            var p1 = cycle[(i - 1 + cycle.Count) % cycle.Count];
                            var p2 = cycle[i];
                            var debt = netMoney.Where(d => d.Debtor == p1 && d.Creditor == p2).Single();
                            logger.WriteLine("\t\t{0} -> {1} ({2:c})", p1, p2, debt.Amount / 100d);
                            cycleTrans.Add(debt);
                        }

                        var subAmount = cycleTrans.Select(c => c.Amount).Min();
                        logger.WriteLine("\t\t{0:c}", subAmount / 100d);
                        foreach (var d in cycleTrans)
                        {
                            d.Amount -= subAmount;
                        }
                    }

                    //remove 0-value debts
                    foreach (var d in netMoney.Where(d => d.Amount == 0).ToList())
                    {
                        netMoney.Remove(d);
                    }
                }
                while (foundCycle);
                logger.WriteLine();
            }

            logger.Flush();
            return netMoney;
        }

        public static List<Tuple<Person, int>> GreatestDebtor(List<Debt> netMoney)
        {
            var ret = new List<Tuple<Person, int>>();
            var people = netMoney.SelectMany(p => new[] { p.Creditor, p.Debtor }).Distinct().ToList();
            foreach (var tup in people
                .Select(p => new { Debtor = p, Owes = netMoney.Where(d => d.Debtor == p).Sum(d => d.Amount) - netMoney.Where(d => d.Creditor == p).Sum(d => d.Amount) })
                .OrderByDescending(obj => obj.Owes))
            {
                ret.Add(new Tuple<Person, int>(tup.Debtor, tup.Owes));
            }
            return ret;
        }

        public static void WriteGraphviz(IEnumerable<Debt> netMoney, TextWriter sw)
        {
            sw.WriteLine("digraph Test {");
            foreach (var d in netMoney)
            {
                sw.WriteLine("\"{0} {1}\" -> \"{2} {3}\" [label=\"{4:c}\"];", d.Debtor.FirstName, d.Debtor.LastName, d.Creditor.FirstName, d.Creditor.LastName, d.Amount / 100d);
            }
            sw.WriteLine("}");
        }

        public static void RenderGraphAsPng(string gvFilePath, string targetFile)
        {
            ProcessStartInfo psi = new ProcessStartInfo("dot", string.Format("-Tpng -o \"{0}\" \"{1}\"", targetFile, gvFilePath));
            var p = Process.Start(psi);
            p.WaitForExit();
        }

        public static HtmlString RenderGraphAsSvg(string gvFileContents)
        {
            var mem = RengerGraphAs(gvFileContents, "svg");
            var doc = XDocument.Load(mem);
            return new HtmlString(doc.Root.ToString(SaveOptions.DisableFormatting));
        }

        private static MemoryStream RengerGraphAs(string gvFileContents, string imgType)
        {
            ProcessStartInfo psi = new ProcessStartInfo("dot", "-T" + imgType);
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            var p = Process.Start(psi);

            p.StandardInput.Write(gvFileContents);
            p.StandardInput.Flush();
			p.StandardInput.Dispose();

            var mem = new MemoryStream();
            p.StandardOutput.BaseStream.CopyTo(mem);
            p.WaitForExit();

            if (p.ExitCode != 0)
                throw new Exception("Graphviz exited with code: " + p.ExitCode);

            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }

        private static List<List<Person>> FindCycles(Person[] people, List<Debt> debts)
        {
            foreach (var p in people)
            {
                p.PrepareForCycleTesting();
            }

            var ret = new List<List<Person>>();

            foreach (var p in people)
            {
                var S = new Stack<Person>();
                CycleDfs(p, people, debts, S, ret);
            }

            return ret;
        }

        private static void CycleDfs(Person v, Person[] verts, List<Debt> edges, Stack<Person> S, List<List<Person>> cycles)
        {
            if (v.Visited == true)
                return;

            v.Visited = true;
            S.Push(v);

            // Consider successors of v
            foreach (var e in edges.Where(e => e.Debtor == v))
            {
                if (S.Contains(e.Creditor))
                {
                    var ret = new List<Person>();
                    foreach (var p in S)
                    {
                        ret.Add(p);
                        if (p == e.Creditor)
                            break;
                    }
                    ret.Reverse();
                    cycles.Add(ret);
                }
                CycleDfs(e.Creditor, verts, edges, S, cycles);
            }

            S.Pop();
        }
    }
}
