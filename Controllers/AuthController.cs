using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
