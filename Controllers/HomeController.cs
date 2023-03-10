using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityNetCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Member, Admin")]
        public IActionResult Member()
        {
            return View();
        }

        // if policy doesn't contain role in services configuration.
        //[Authorize(Roles = "Admin")]
        //[Authorize(Policy = "Department")]

        // if policy contains role in services configuration, and role will be automatically applied.
        [Authorize(Policy = "DpmtAdmin")]
        public IActionResult Admin()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
