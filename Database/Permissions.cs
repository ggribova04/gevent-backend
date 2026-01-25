using System.ComponentModel.DataAnnotations;

public class Permission
{
    [Key]
    public int IdPermission { get; set; }

    [Required]
    [MaxLength(255)]
    public string ComponentName { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; }
}