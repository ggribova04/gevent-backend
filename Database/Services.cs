using System.ComponentModel.DataAnnotations;

public class Service
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public int SupplierId { get; set; }
    public User Supplier { get; set; }

    [Required]
    public int EventId { get; set; }
    public Event Event { get; set; }

    [Required]
    public int IdStatus { get; set; }
    public TaskStatus Status { get; set; }
}