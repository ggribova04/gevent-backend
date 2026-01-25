using System.ComponentModel.DataAnnotations;

public class Role
{
    [Key]
    public int IdRole { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public ICollection<User> Users { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
    public ICollection<Organization> Organizations { get; set; }
}
