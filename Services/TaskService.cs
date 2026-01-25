using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TaskService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TaskDto>> GetTasksByEventAsync(int eventId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.EventId == eventId)
            .Include(t => t.Employee) // Подгружаем связанного сотрудника
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IdEvent = t.EventId,
                Date = DateOnly.FromDateTime(t.Date),
                IdEmployee = t.EmployeeId,
                IdStatus = t.IdStatus,
                EmployeeFullName = t.Employee.FullName, // Форматируем ФИО
                Login = t.Employee.UserName, // Логин из User
                Status = t.Status.Name
            })
            .ToListAsync();

        return tasks;
    }

    public async Task<bool> CreateTaskAsync(string login, TaskDto dto)
    {
        var employee = await _context.Users.FirstOrDefaultAsync(u => u.UserName == login && u.IdRole == 3);
        if (employee == null)
            return false;

        // Добавление в таблицу Organizations, если ещё не добавлен
        bool alreadyInOrganization = await _context.Organizations
            .AnyAsync(o => o.UserId == employee.Id && o.EventId == dto.IdEvent);

        if (!alreadyInOrganization)
        {
            _context.Organizations.Add(new Organization
            {
                UserId = employee.Id,
                EventId = dto.IdEvent,
                IdRole = 3
            });
        }

        var task = _mapper.Map<Task>(dto);
        task.EmployeeId = employee.Id;
        task.Date = DateTime.SpecifyKind(dto.Date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UpdateTaskStatusDto> UpdateTaskStatusAsync(UpdateTaskStatusDto dto)
    {
        var task = await _context.Tasks.FindAsync(dto.IdTask);
        if (task == null)
            throw new Exception("Задача не найдена");

        task.IdStatus = dto.IdStatus;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        return dto;
    }

    public async Task<List<TaskDto>> GetAllUserTasksAsync(int userId, int userRoleId)
    {
        var result = new List<TaskDto>();

        if (userRoleId == 1)
        {
            // Администратор: получить все задачи, где он — организатор
            var eventIds = await _context.Events
                .Where(e => e.OrganizerId == userId)
                .Select(e => e.Id)
                .ToListAsync();

            var employeeTasks = await _context.Tasks
                .Where(t => eventIds.Contains(t.EventId))
                .Include(t => t.Employee)
                .Include(t => t.Status)
                .Include(t => t.Event)
                .ToListAsync();

            var performerTasks = await _context.Services
                .Where(s => eventIds.Contains(s.EventId))
                .Include(s => s.Supplier)
                .Include(s => s.Status)
                .Include(s => s.Event)
                .ToListAsync();

            result.AddRange(employeeTasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title + " - " + t.Event.Title,
                Description = t.Description,
                IdEvent = t.EventId,
                Date = DateOnly.FromDateTime(t.Date),
                IdEmployee = t.EmployeeId,
                IdStatus = t.IdStatus,
                Status = t.Status.Name
            }));
        }

        else if (userRoleId == 3)
        {
            // Сотрудник: задачи из Tasks, где EmployeeId = userId
            var employeeTasks = await _context.Tasks
                .Where(t => t.EmployeeId == userId)
                .Include(t => t.Employee)
                .Include(t => t.Status)
                .Include(t => t.Event)
                .ToListAsync();

            result.AddRange(employeeTasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = "Мероприятие: " + t.Event.Title + "Описание задачи: " + t.Description,
                IdEvent = t.EventId,
                Date = DateOnly.FromDateTime(t.Date),
                IdEmployee = t.EmployeeId,
                IdStatus = t.IdStatus,
                Status = t.Status.Name,
                Type = "task"
            }));
        }

        return result;
    }

}
