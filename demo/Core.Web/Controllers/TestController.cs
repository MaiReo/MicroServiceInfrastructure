using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Web.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet()]
        public async Task<ActionResult<string>> GetTest()
        {
            await Task.Run(() => { });

            return "test";
        }
    }
}
