using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Transactions;
using System.Web.Http;
using Austin.DkpLib;

namespace DkpWeb.Areas.API.Controllers
{
    [DataContract]
    public class ApiBillSplit
    {
        public ApiBillSplit(BillSplit bs)
        {
            this.ID = bs.ID;
            this.Name = bs.Name;
        }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
    public class BillSplitController : ApiController
    {
        // GET api/billsplit
        public IEnumerable<ApiBillSplit> Get()
        {
            using (var db = new DkpDataContext())
            {
                return db.BillSplits.Select(bs => new ApiBillSplit(bs)).ToList();
            }
        }

        // GET api/billsplit/5
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/billsplit
        public void Post([FromBody]ApiTransaction[] newTransactions)
        {
            using (var db = new DkpDataContext())
            {
                using (var ts = new TransactionScope())
                {
                    var bs = new BillSplit();
                    bs.Name = newTransactions[0].Description;
                    db.BillSplits.InsertOnSubmit(bs);
                    db.Transactions.InsertAllOnSubmit(newTransactions.Select(t => t.ToTransaction(bs)));
                    db.SubmitChanges();
                    ts.Complete();
                }
            }
        }

        // PUT api/billsplit/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/billsplit/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
