using System.ComponentModel.DataAnnotations;

public class TaskStatus
{
    [Key]
    public int IdStatus { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    public ICollection<Task> Tasks { get; set; }
    public ICollection<Service> Services { get; set; }
}
