using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers
{
    public class IdentityUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
