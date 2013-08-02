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
    public class ApiPerson
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }
    public class PeopleController : ApiController
    {
        // GET api/people
        public IEnumerable<ApiPerson> Get()
        {
            using (var db = new DkpDataContext())
            {
                return db.People.Select(p => new ApiPerson() { ID = p.ID, FirstName = p.FirstName, LastName = p.LastName }).ToList();
            }
        }

        // GET api/people/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/people
        public void Post([FromBody]string value)
        {
        }

        // PUT api/people/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/people/5
        public void Delete(int id)
        {
        }
    }
}
