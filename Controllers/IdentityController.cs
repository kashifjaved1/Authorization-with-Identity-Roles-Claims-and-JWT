using IdentityNetCore.Data;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            //ViewBag.AllRoles
            ViewData["AllRoles"] = await _roleManager.Roles.ToListAsync();
            return View();
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
                        await _userManager.AddToRoleAsync(user, signUp.Role);
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

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Identity");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            // getting and setting user_role(s) into UserRoleDTO
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserRolesDTO>();
            foreach (var user in users)
            {
                userRoles.Add(new UserRolesDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            return View(userRoles);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("GetUsers", "Identity");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetRoles()
        {
            var roleList = _roleManager.Roles.ToList();
            var roles = new List<RoleDTO>();

            if(roleList != null)
            {
                foreach (var role in roleList)
                {
                    roles.Add(new RoleDTO
                    {
                        Id = role.Id,
                        Name = role.Name
                    });
                }

                return View(roles);
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            /*var id = roleDTO.Id*/;
            var role = await _roleManager.FindByIdAsync(id);
            //var isRoleExist = await _roleManager.RoleExistsAsync(name);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            return RedirectToAction("GetRoles", "Identity");
        }
    }
}
