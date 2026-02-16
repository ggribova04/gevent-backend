using gevent.Database.Enums;
using System.ComponentModel.DataAnnotations;

public class Tender
{
  [Key]
  public int Id { get; set; }

  [Required]
  public int EventId { get; set; }
  public Event Event { get; set; }

  [Required]
  [MaxLength(255)]
  public string Title { get; set; }

  [Required]
  [MaxLength(100)]
  public string City { get; set; }

  [Required]
  [MaxLength(255)]
  public string ServiceName { get; set; }

  [Required]
  public DateOnly Deadline { get; set; }

  [MaxLength(255)]
  public string Contacts { get; set; }

  public string Comment { get; set; }

  [Required]
  public TenderState Status { get; set; }

  public ICollection<TenderResponse> Responses { get; set; }
}
