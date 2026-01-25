using System.ComponentModel.DataAnnotations;

public class EventStatus
{
    [Key]
    public int IdStatus { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    public ICollection<Event> Events { get; set; }
}
