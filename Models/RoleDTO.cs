namespace IdentityNetCore.Models
{
    public class RoleDTO
    {
        public int Id { get; set; }
    }

    public class CreateRoleDTO
    {
        public string Name { get; set; }
    }

    public class DeleteRoleDTO : RoleDTO
    {
        //
    }
}
