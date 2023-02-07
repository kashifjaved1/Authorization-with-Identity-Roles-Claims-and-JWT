using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IdentityNetCore.Models
{
    public class SignInDTO
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Valid Password is Required")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class SignUpDTO : SignInDTO
    {        
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid Email Address is Required.")]
        public string Email { get; set; }
        public string Department { get; set; }

        [Required]
        public string Role { get; set; }
        //public List<IdentityRole> Roles { get; set; }
    }

    public class UserDTO : SignUpDTO
    {
        public string Id { get; set; }
    }

    public class UserRolesDTO : UserDTO
    {
        public IList<Claim> Claims { get; set; }
        public IList<string> Roles { get; set; }
    }
}
