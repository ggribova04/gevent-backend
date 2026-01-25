using System.ComponentModel.DataAnnotations;

public class EventGuest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }
    public Event Event { get; set; }

    [Required]
    public string GuestInfo { get; set; }
}