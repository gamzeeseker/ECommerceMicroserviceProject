using Microsoft.AspNetCore.Mvc;

namespace OrderServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
    }
}
