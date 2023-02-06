using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityNetCore.Models
{
    public class CreateRoleDTO
    {
        public string Name { get; set; }
    }

    public class RoleDTO : CreateRoleDTO
    {
        public string Id { get; set; }
    }

    //public class DeleteRoleDTO
    //{
    //    public List<RoleDTO> Roles { get; set; }
    //    //public List<IdentityRole> Roles { get; set; }
    //}
}
