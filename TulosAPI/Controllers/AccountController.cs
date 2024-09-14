using Microsoft.AspNetCore.Mvc;

namespace TulosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
