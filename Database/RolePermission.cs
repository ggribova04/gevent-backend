using System.ComponentModel.DataAnnotations;

public class RolePermission
{
    public int IdRole { get; set; }
    public Role Role { get; set; }

    public int IdPermission { get; set; }
    public Permission Permission { get; set; }

    public bool CanAccess { get; set; }
}
