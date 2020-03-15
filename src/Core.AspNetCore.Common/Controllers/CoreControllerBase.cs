using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomOutputFormatter]
    public abstract class CoreControllerBase : ControllerBase
    {
        protected CancellationToken RequestAborted => HttpContext?.RequestAborted ?? default;
    }
}