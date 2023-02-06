using IdentityNetCore.Data;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityNetCore.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<SystemUser> _userManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityController(UserManager<SystemUser> userManager, SignInManager<SystemUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> SignUp()
        {
            var roles = new SignUpDTO
            {
                Roles = _roleManager.Roles.ToList()
            };

            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDTO signUp)
        {
            if(ModelState.IsValid)
            {
                if(await _userManager.FindByEmailAsync(signUp.Email) == null)
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
                var result = await _signInManager.PasswordSignInAsync(signIn.FullName, signIn.Password, isPersistent: true, false);

                if(result.Succeeded) return RedirectToAction("Index", "Home");
                ModelState.AddModelError("Login", "Can't login with this boi.");
            }

            return View(signIn);
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleDTO createRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isExist = await _roleManager.FindByNameAsync(createRole.Name);

                    if (isExist == null)
                    {
                        var role = new IdentityRole { Name = createRole.Name };
                        var result = await _roleManager.CreateAsync(role);

                        if(!result.Succeeded)
                        {
                            var errors = result.Errors.Select(x => x.Description);
                            ModelState.AddModelError("Role", string.Join(",", errors));
                            return View(createRole);
                        } 
                    }

                    return RedirectToAction("SignUp", "Identity");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Role", "Something wrong with data!");
            }
            
            return View(createRole);
        }
    }
}
