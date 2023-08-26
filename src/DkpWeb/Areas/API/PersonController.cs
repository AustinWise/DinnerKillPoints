using Austin.DkpLib;
using DkpWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DkpWeb.Areas.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationDbContext mData;

        public PersonController(ApplicationDbContext data)
        {
            this.mData = data;
        }

        [HttpGet]
        public async Task<List<SplitPerson>> Get()
        {
            return await mData.Person.Select(p => new SplitPerson()
            {
                Id = p.Id,
                FullName = p.FullName,
            }).ToListAsync();
        }
    }
}
