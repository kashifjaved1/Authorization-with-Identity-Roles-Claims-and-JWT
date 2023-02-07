using IdentityNetCore.Data;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityNetCore.Controllers
{
    public class IdentityRoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityRoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
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

                        if (!result.Succeeded)
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
        public IActionResult GetRoles()
        {
            var roleList = _roleManager.Roles.ToList();
            var roles = new List<RoleDTO>();

            if (roleList != null)
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
            /*var id = roleDTO.Id*/
            ;
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
