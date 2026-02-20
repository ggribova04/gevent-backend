using gevent.Database.Enums;

public class TenderDto
{
  public int Id { get; set; }

  public string Title { get; set; }
  public string City { get; set; }
  public string ServiceName { get; set; }
  public DateOnly Deadline { get; set; }

  public int EventId { get; set; }
  public string EventTitle { get; set; }
  public DateOnly EventDate { get; set; }
  public TimeOnly EventTime { get; set; }
  public string OrganizerEvent { get; set; }

  public string Contacts { get; set; }
  public string Comment { get; set; }

  public TenderState Status { get; set; }

  public TenderViewStatus ViewStatus { get; set; }
}

public class CreateTenderRequest
{
  public int EventId { get; set; }
  public string Title { get; set; }
  public string City { get; set; }
  public string ServiceName { get; set; }
  public DateOnly Deadline { get; set; }
  public string Contacts { get; set; }
  public string Comment { get; set; }
}

public class CreateTenderResponseRequest
{
  public decimal CostService { get; set; }
  public string Contacts { get; set; }
  public string Comment { get; set; }
}

public class TenderResponseDto
{
  public int Id { get; set; }
  public int TenderId { get; set; }
  public int EmployeeId { get; set; }
  public string EmployeeFullName { get; set; }
  public string EmployeeEmail { get; set; }
  public string EmployeeSpecialization { get; set; }
  public string? EmployeeDescription { get; set; }
  public string Status { get; set; }
  public decimal CostService { get; set; }
  public string Contacts { get; set; }
  public string Comment { get; set; }
}
