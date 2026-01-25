using System.ComponentModel.DataAnnotations;

public class Task
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    public string Description { get; set; }

    [Required]
    public int EventId { get; set; }
    public Event Event { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int IdStatus { get; set; }
    public TaskStatus Status { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public User Employee { get; set; }
}
