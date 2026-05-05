using System.ComponentModel.DataAnnotations;
using gevent.Database.Enums;

public class Task
{
  [Key]
  public int Id { get; set; }

  [Required]
  public int EventId { get; set; }
  public Event Event { get; set; }

  public int EmployeeId { get; set; }
  public User Employee { get; set; }

  [Required]
  [MaxLength(255)]
  public string Title { get; set; }

  public string Description { get; set; }

  [Required]
  public DateOnly Deadline { get; set; }

  [Required]
  public TaskState Status { get; set; }

  public int? TenderId { get; set; }
  public Tender? Tender { get; set; }
}
