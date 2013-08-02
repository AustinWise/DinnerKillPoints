using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;
using Austin.DkpLib;

namespace DkpWeb.Areas.API.Controllers
{
    [DataContract]
    public class ApiTransaction
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public int Amount { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public int DebtorId { get; set; }
        [DataMember]
        public int CreditorId { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
    public class TransactionController : ApiController
    {
        // GET api/transaction
        public IEnumerable<ApiTransaction> Get()
        {
            using (var db = new DkpDataContext())
            {
                return db.Transactions.Select(t => new ApiTransaction
                {
                    ID = t.ID,
                    Amount = t.Amount,
                    Created = t.Created,
                    DebtorId = t.DebtorID,
                    CreditorId = t.CreditorID,
                    Description = t.Description
                }).ToList();
            }
        }

        // GET api/transaction/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/transaction
        public void Post([FromBody]string value)
        {
        }

        // PUT api/transaction/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/transaction/5
        public void Delete(int id)
        {
        }
    }
}
