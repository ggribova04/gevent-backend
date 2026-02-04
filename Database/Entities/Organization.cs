using System.ComponentModel.DataAnnotations;

public class Organization
{
  [Key]
  public int Id { get; set; }

  [Required]
  public int EventId { get; set; }
  public Event Event { get; set; }

  [Required]
  public int UserId { get; set; }
  public User User { get; set; }

  [Required]
  public int IdRole { get; set; }
  public Role Role { get; set; }
}
