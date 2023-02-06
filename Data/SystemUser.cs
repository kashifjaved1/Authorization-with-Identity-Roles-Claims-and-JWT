using Microsoft.AspNetCore.Identity;

namespace IdentityNetCore.Data
{
    public class SystemUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
