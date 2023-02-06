using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers
{
    public class IdentityRoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
