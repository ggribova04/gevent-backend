using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/events")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskController(ITaskService taskService, IHttpContextAccessor httpContextAccessor)
    {
        _taskService = taskService;
        _httpContextAccessor = httpContextAccessor;
    }

    private int? GetCurrentEventId()
    {
        return HttpContext.Session.GetInt32("CurrentEventId");
    }

    [HttpGet("create/step3")]

    public async Task<IActionResult> GetByEvent()
    {
        var eventId = GetCurrentEventId();
        if (eventId == null)
            return BadRequest("ID мероприятия не найден в текущей сессии");

        var tasks = await _taskService.GetTasksByEventAsync(eventId.Value);
        return Ok(tasks);
    }

    [HttpPost("create/step3")]
    public async Task<IActionResult> Create([FromBody] TaskCreateRequest request)
    {
        var sessionEventId = GetCurrentEventId();
        var finalEventId = sessionEventId ?? request.IdEvent;

        if (finalEventId == null || finalEventId == 0)
            return BadRequest("ID мероприятия не найден ни в сессии, ни в параметрах запроса");

        var dto = new TaskDto
        {
            Title = request.Title,
            Description = request.Description,
            Date = request.Date,
            IdEvent = finalEventId,
            IdStatus = 0 // или другой статус по умолчанию
        };

        var success = await _taskService.CreateTaskAsync(request.Login, dto);
        return success ? Ok() : BadRequest("Сотрудник не найден");
    }


    [HttpPut("create/step3/update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTaskStatusDto dto)
    {
        try
        {
            var result = await _taskService.UpdateTaskStatusAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{eventId}/tasks")]
    public async Task<IActionResult> GetTasks(int eventId)
    {
        var tasks = await _taskService.GetTasksByEventAsync(eventId);
        // Возвращаем пустой массив, если задач нет
        return Ok(tasks ?? new List<TaskDto>());
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> GetMyTasks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleIdClaim = User.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null || roleIdClaim == null)
            return Unauthorized("Claims not found");

        var userId = int.Parse(userIdClaim.Value);
        var roleId = int.Parse(roleIdClaim.Value);

        var tasks = await _taskService.GetAllUserTasksAsync(userId, roleId);
        return Ok(tasks);
    }
}

