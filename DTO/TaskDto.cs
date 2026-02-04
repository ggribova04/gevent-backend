using gevent.Database.Enums;
using System.ComponentModel.DataAnnotations;

public class TaskDto
{
  public int Id { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public int EventId { get; set; }
  public string EventTitle { get; set; }
  public DateOnly EventDate { get; set; }
  public string OrganizerEvent { get; set; }
  public DateOnly Deadline { get; set; }
  public int EmployeeId { get; set; }
  public string Status { get; set; }
  public string EmployeeFullName { get; set; }
  public string EmployeeLogin { get; set; }
}

public class TaskCreateRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly Deadline { get; set; }
    public string EmployeeLogin { get; set; }
    public int EventId { get; set; }
}

public class UpdateTaskStatusDto
{
    public int TaskId { get; set; }
    public TaskState Status { get; set; }
}

public class TaskDeleteRequest
{
  public int TaskId { get; set; }
}
