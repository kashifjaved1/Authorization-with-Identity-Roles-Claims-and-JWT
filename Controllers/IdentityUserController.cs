using IdentityNetCore.Data;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace IdentityNetCore.Controllers
{
    public class IdentityUserController : Controller
    {
        private readonly UserManager<SystemUser> _userManager;

        public IdentityUserController(UserManager<SystemUser> userManager)
        {
            _userManager = userManager;
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
                    Claims = await _userManager.GetClaimsAsync(user),
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

            return RedirectToAction("GetUsers");
        }
    }
}
