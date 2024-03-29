﻿using Austin.DkpLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DkpWeb.Areas.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "DKP")]
    public class BillSplitController : ControllerBase
    {
        private readonly IBillSplitterServices mData;

        public BillSplitController(IBillSplitterServices data)
        {
            this.mData = data;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] BillSplitResult result)
        {
            await mData.SaveBillSplitResult(result);
            return Ok();
        }
    }
}
