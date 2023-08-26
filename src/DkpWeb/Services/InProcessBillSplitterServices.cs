using Austin.DkpLib;
using DkpWeb.Data;
using DkpWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DkpWeb.Services
{
    public sealed class InProcessBillSplitterServices : IBillSplitterServices
    {
        readonly ApplicationDbContext mData;

        public InProcessBillSplitterServices(ApplicationDbContext data)
        {
            this.mData = data;
        }

        public async Task<SplitPerson[]> GetAllPeopleAsync()
        {
            return await mData.ActivePeopleOrderedByName.Select(p => new SplitPerson()
            {
                Id = p.Id,
                FullName = p.FullName,
            }).ToArrayAsync();
        }

        public async Task SaveBillSplitResult(BillSplitResult result)
        {
            var transToCreate = new List<Transaction>();
            DateTime created = DateTime.UtcNow;
            var bs = new BillSplit()
            {
                Name = result.Name,
            };
            foreach (var t in result.Transactions)
            {
                if (t.Amount < Money.Zero)
                    throw new ArgumentOutOfRangeException();
                transToCreate.Add(new Transaction()
                {
                    CreditorId = t.CreditorId,
                    Amount = t.Amount,
                    DebtorId = t.DebtorId,
                    Description = result.Name,
                    Created = created,
                    Id = Guid.NewGuid(),
                    Bill = bs,
                });;
            }

            using var dbtrans = await mData.Database.BeginTransactionAsync();
            mData.BillSplit.Add(bs);
            mData.Transaction.AddRange(transToCreate);
            await mData.SaveChangesAsync();
            await dbtrans.CommitAsync();
        }
    }
}
