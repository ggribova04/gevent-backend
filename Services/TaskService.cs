using AutoMapper;
using gevent.Database.Entities;
using gevent.Database.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class TaskService : ITaskService
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;
  private readonly IHubContext<TasksHub> _hubContextTasks;
  private readonly IHubContext<TendersHub> _hubContextTenders;

  public TaskService(ApplicationDbContext context, IMapper mapper, IHubContext<TasksHub> hubContextTasks, IHubContext<TendersHub> hubContextTenders)
  {
    _context = context;
    _mapper = mapper;
    _hubContextTasks = hubContextTasks;
    _hubContextTenders = hubContextTenders;
  }

  public async Task<List<TaskDto>> GetStep2TasksAsync(int eventId)
  {
    return await _context.Tasks
        .Where(t => t.EventId == eventId)
        .Include(t => t.Employee)
        .Where(t => t.Employee.RoleId == 2)
        .Select(t => new TaskDto
        {
          Id = t.Id,
          Title = t.Title,
          Deadline = t.Deadline,
          EmployeeFullName = t.Employee.FullName,
          EmployeeLogin = t.Employee.UserName,
          Status = t.Status.ToString()
        })
        .ToListAsync();
  }

  public async Task<List<TaskDto>> GetStep3TasksAsync(int eventId)
  {
    return await _context.Tasks
        .Where(t => t.EventId == eventId)
        .Include(t => t.Employee)
        .Where(t => t.Employee.RoleId == 3 || t.Employee.RoleId == 1)
        .Select(t => new TaskDto
        {
          Id = t.Id,
          Title = t.Title,
          Deadline = t.Deadline,
          EmployeeFullName = t.Employee.FullName,
          EmployeeLogin = t.Employee.UserName,
          Status = t.Status.ToString()
        })
        .ToListAsync();
  }

  public async Task<List<TaskDto>> GetTasksByEventAsync(int eventId)
  {
    var tasks = await _context.Tasks
        .Where(t => t.EventId == eventId)
        .Include(t => t.Employee)
        .Select(t => new TaskDto
        {
          Id = t.Id,
          Title = t.Title,
          Description = t.Description,
          EventId = t.EventId,
          Deadline = t.Deadline,
          EmployeeId = t.EmployeeId,
          EmployeeFullName = t.Employee.FullName,
          EmployeeLogin = t.Employee.UserName,
          Status = t.Status.ToString()
        })
        .ToListAsync();

    return tasks;
  }

  public async Task<bool> CreateTaskAsync(string login, int[] allowedRoleIds, TaskDto dto)
  {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.UserName == login);

    if (user == null)
      return false;

    if (!allowedRoleIds.Contains(user.RoleId))
      return false;

    bool alreadyInOrganization = await _context.Organizations
        .AnyAsync(o => o.UserId == user.Id && o.EventId == dto.EventId);

    if (!alreadyInOrganization)
    {
      _context.Organizations.Add(new Organization
      {
        UserId = user.Id,
        EventId = dto.EventId,
        IdRole = user.RoleId
      });
    }

    var task = _mapper.Map<Task>(dto);
    task.EmployeeId = user.Id;

    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    await _hubContextTasks.Clients.All.SendAsync("TasksUpdated");

    return true;
  }

  public async Task<UpdateTaskStatusDto> UpdateTaskStatusAsync(UpdateTaskStatusDto dto)
  {
    var task = await _context.Tasks.FindAsync(dto.TaskId);
    if (task == null)
      throw new Exception("Задача не найдена");

    task.Status = dto.Status;

    _context.Tasks.Update(task);
    await _context.SaveChangesAsync();

    await _hubContextTasks.Clients.All.SendAsync("TasksUpdated");

    return dto;
  }

  public async Task<List<TaskDto>> GetAllUserTasksAsync(int userId, int userRoleId)
  {
    var hiddenTaskIds = await _context.UserTaskVisibilities
      .Where(x => x.UserId == userId)
      .Select(x => x.TaskId)
      .ToListAsync();

    var result = new List<TaskDto>();

    if (userRoleId == 1)
    {
      var eventIds = await _context.Events
        .Where(e => e.OrganizerId == userId)
        .Select(e => e.Id)
        .ToListAsync();

      var employeeTasks = await _context.Tasks
        .Where(t =>
          (eventIds.Contains(t.EventId) || t.EmployeeId == userId) &&
          !hiddenTaskIds.Contains(t.Id))
        .Include(t => t.Employee)
        .Include(t => t.Event)
          .ThenInclude(e => e.Organizer)
        .ToListAsync();

      result.AddRange(employeeTasks.Select(t => new TaskDto
      {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        EventId = t.EventId,
        EventTitle = t.Event.Title,
        EventDate = t.Event.Date,
        OrganizerEvent = t.Event.Organizer.FullName,
        Deadline = t.Deadline,
        EmployeeId = t.EmployeeId,
        EmployeeFullName = t.Employee.FullName,
        EmployeeLogin = t.Employee.UserName,
        Status = t.Status.ToString()
      }));
    }
    else if (userRoleId == 2 || userRoleId == 3)
    {
      var employeeTasks = await _context.Tasks
        .Where(t =>
          t.EmployeeId == userId &&
          !hiddenTaskIds.Contains(t.Id))
        .Include(t => t.Employee)
        .Include(t => t.Event)
          .ThenInclude(e => e.Organizer)
        .ToListAsync();

      result.AddRange(employeeTasks.Select(t => new TaskDto
      {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        EventId = t.EventId,
        EventTitle = t.Event.Title,
        EventDate = t.Event.Date,
        OrganizerEvent = t.Event.Organizer.FullName,
        Deadline = t.Deadline,
        EmployeeId = t.EmployeeId,
        Status = t.Status.ToString(),
      }));
    }

    return result;
  }

  public async System.Threading.Tasks.Task HideTaskAsync(int taskId, int userId)
  {
    bool alreadyHidden = await _context.UserTaskVisibilities
      .AnyAsync(x => x.UserId == userId && x.TaskId == taskId);

    if (alreadyHidden)
      return;

    var hiddenTask = new UserTaskVisibility
    {
      UserId = userId,
      TaskId = taskId
    };

    _context.UserTaskVisibilities.Add(hiddenTask);

    await _context.SaveChangesAsync();

    await _hubContextTasks.Clients.All.SendAsync("TasksUpdated");
  }

  public async Task<bool> DeleteTaskAsync(int taskId)
  {
    var task = await _context.Tasks.FindAsync(taskId);
    if (task == null) return false;

    _context.Tasks.Remove(task);
    await _context.SaveChangesAsync();
    await _hubContextTasks.Clients.All.SendAsync("TasksUpdated");

    return true;
  }

  public async Task<List<User>> SearchPerformersAsync(PerformerSearchDto dto)
  {
    var query = _context.Users.AsQueryable();

    query = query.Where(u => u.RoleId == 2);

    if (!string.IsNullOrWhiteSpace(dto.Specialization))
    {
      query = query.Where(u => u.Specialization != null &&
                               u.Specialization.ToLower().Contains(dto.Specialization.ToLower()));
    }

    if (!string.IsNullOrWhiteSpace(dto.City))
    {
      query = query.Where(u => u.City != null &&
                               u.City.ToLower().Contains(dto.City.ToLower()));
    }

    return await query.ToListAsync();
  }

  public async System.Threading.Tasks.Task CreateTaskFromTenderAsync(int tenderId, int employeeId)
  {
    var response = await _context.TenderResponses
      .Include(r => r.Tender)
      .FirstOrDefaultAsync(r =>
        r.TenderId == tenderId &&
        r.EmployeeId == employeeId &&
        r.Status == TenderResponseState.Won);

    if (response == null)
      throw new InvalidOperationException("Вы не победитель этого конкурса");

    var tender = response.Tender;

    var task = new Task
    {
      Title = tender.Title,
      Description = BuildDescription(tender, response),
      EventId = tender.EventId,
      TenderId = tender.Id,
      Deadline = tender.Deadline,
      EmployeeId = employeeId,
      Status = TaskState.InProgress
    };

    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    await _hubContextTasks.Clients.All.SendAsync("TasksUpdated");
    await _hubContextTenders.Clients.All.SendAsync("TendersUpdated");
  }

  private static string BuildDescription(
    Tender tender,
    TenderResponse winner
  )
  {
    return
      $"""
       Контакты заказчика:
       {tender.Contacts}

       Комментарий заказчика:
       {tender.Comment}

       Стоимость:
       {winner.CostService}
       """;
  }
}
