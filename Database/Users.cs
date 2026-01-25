using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; }

    [Required]
    [MaxLength(255)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    public int IdRole { get; set; }
    public Role Role { get; set; }

    [MaxLength(255)]
    public string? Specialization { get; set; } // доп. поле для исполнителя

    [MaxLength(100)]
    public string? City { get; set; } // доп. поле для исполнителя

    [Required]
    public string PasswordHash { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(5000)]
    public string? PhotoUrl { get; set; }

    public ICollection<Event> Events { get; set; }
    public ICollection<Organization> Organizations { get; set; }
    public ICollection<Service> Services { get; set; }
    public ICollection<Task> Tasks { get; set; }
    public ICollection<EventGuest> EventGuests { get; set; }
}