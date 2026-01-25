using System.ComponentModel.DataAnnotations;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int IdEvent { get; set; }
    public DateOnly Date {  get; set; }
    public int IdEmployee { get; set; }
    public int IdStatus { get; set; }
    public string? Status { get; set; }
    public string EmployeeFullName { get; set; } // ФИО сотрудника
    public string Login { get; set; } // Логин сотрудника
    public string Type {  get; set; }
}

public class TaskCreateRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly Date { get; set; }
    public string Login { get; set; }
    public int IdEvent { get; set; }
}

public class TaskStatusDto
{
    public int IdStatus { get; set; }
    public string Name { get; set; }
}

public class TaskStatusWithRelationsDto : TaskStatusDto
{
    public IEnumerable<TaskDto> Tasks { get; set; }
    public IEnumerable<ServiceDto> Services { get; set; }
}

public class UpdateTaskStatusDto
{
    public int IdTask { get; set; }
    public int IdStatus { get; set; }
}

