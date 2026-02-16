using System.ComponentModel.DataAnnotations;

public class TenderResponse
{
  [Key]
  public int Id { get; set; }

  [Required]
  public int TenderId { get; set; }
  public Tender Tender { get; set; }

  [Required]
  public int EmployeeId { get; set; }
  public User Employee { get; set; }

  [Required]
  public decimal CostService { get; set; }

  [MaxLength(255)]
  public string Contacts { get; set; }

  public string Comment { get; set; }

  [Required]
  public TenderResponseState Status { get; set; }
}
