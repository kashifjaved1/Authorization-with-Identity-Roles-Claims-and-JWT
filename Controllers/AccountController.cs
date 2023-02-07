using IdentityNetCore.Data;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityNetCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<SystemUser> _userManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<SystemUser> userManager, SignInManager<SystemUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> SignUp()
        {
            //ViewBag.AllRoles
            ViewData["AllRoles"] = await _roleManager.Roles.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDTO signUp)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(signUp.Email) == null)
                {
                    var user = new SystemUser
                    {
                        Email = signUp.Email,
                        UserName = signUp.FullName,
                        FullName = signUp.FullName
                    };

                    var result = await _userManager.CreateAsync(user, signUp.Password);
                    user = await _userManager.FindByEmailAsync(signUp.Email);

                    if (result.Succeeded)
                    {
                        var claim = new Claim("Department", signUp.Department);
                        await _userManager.AddToRoleAsync(user, signUp.Role);
                        await _userManager.AddClaimAsync(user, claim);
                        return RedirectToAction("SignIn");
                    }

                    ModelState.AddModelError("Signup", string.Join(", ", result.Errors.Select(x => x.Description)));
                    return View(signUp);
                }
            }

            return View(signUp);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDTO signIn)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(signIn.FullName, signIn.Password, isPersistent: signIn.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(signIn.FullName);
                    if (await _userManager.IsInRoleAsync(user, "Admin")) return RedirectToAction("Admin", "Home");
                    if (await _userManager.IsInRoleAsync(user, "Member")) return RedirectToAction("Member", "Home");
                }

                ModelState.AddModelError("Login", "Can't login with this boi.");
            }

            return View(signIn);
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
