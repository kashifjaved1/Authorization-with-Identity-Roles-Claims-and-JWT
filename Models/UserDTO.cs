using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityNetCore.Models
{
    public class UserDTO
    {
        //
    }

    public class SignInDTO : UserDTO
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Valid Password is Required")]
        public string Password { get; set; }
    }

    public class SignUpDTO : SignInDTO
    {        
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid Email Address is Required.")]
        public string Email { get; set; }

        [Required]
        //public string Role { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
