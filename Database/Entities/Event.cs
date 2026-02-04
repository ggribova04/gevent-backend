using gevent.Database.Enums;
using System.ComponentModel.DataAnnotations;

public class Event
{
  [Key]
  public int Id { get; set; }

  [Required]
  [MaxLength(255)]
  public string Title { get; set; }

  [Required]
  public DateOnly Date { get; set; }

  [Required]
  public TimeOnly Time { get; set; }

  [Required]
  public EventState Status { get; set; }

  [Required]
  public int OrganizerId { get; set; }
  public User Organizer { get; set; }

  public ICollection<Task> Tasks { get; set; }
  public ICollection<Tender> Tenders { get; set; }
  public ICollection<Organization> Organizations { get; set; }
  public ICollection<EventGuest> EventGuests { get; set; }
}
