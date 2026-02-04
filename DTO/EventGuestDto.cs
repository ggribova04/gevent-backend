using System.ComponentModel.DataAnnotations;

public class EventGuestDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string GuestInfo { get; set; }
}

public class AddGuestRequest
{
    [Required]
    public string GuestInfo { get; set; }
    public int EventId { get; set; }
}
