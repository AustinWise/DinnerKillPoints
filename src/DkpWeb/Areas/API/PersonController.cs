﻿using Austin.DkpLib;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DkpWeb.Areas.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IBillSplitterServices mData;

        public PersonController(IBillSplitterServices data)
        {
            this.mData = data;
        }

        [HttpGet]
        public async Task<SplitPerson[]> Get()
        {
            return await mData.GetAllPeopleAsync();
        }
    }
}
