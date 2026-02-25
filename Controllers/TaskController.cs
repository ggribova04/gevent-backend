using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using gevent.Database.Enums;

[Authorize]
[ApiController]
[Route("api/events")]
public class TaskController : ControllerBase
{
  private readonly ITaskService _taskService;
  private readonly IHttpContextAccessor _httpContext;

  public TaskController(ITaskService taskService, IHttpContextAccessor httpContext)
  {
    _taskService = taskService;
    _httpContext = httpContext;
  }

  private int GetUserId() => int.Parse(_httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

  private int? GetCurrentEventId()
  {
    return HttpContext.Session.GetInt32("CurrentEventId");
  }

  private async Task<IActionResult> CreateInternal(TaskCreateRequest request, int roleId)
  {
    var sessionEventId = GetCurrentEventId();
    var finalEventId = sessionEventId ?? request.EventId;

    if (finalEventId == null || finalEventId == 0)
      return BadRequest("ID мероприятия не найден");

    var dto = new TaskDto
    {
      Title = request.Title,
      Description = request.Description,
      Deadline = request.Deadline,
      EventId = finalEventId,
      Status = TaskState.NotAccepted.ToString()
    };

    var success = await _taskService.CreateTaskAsync(request.EmployeeLogin, roleId, dto);
    return success ? Ok() : BadRequest("Пользователь не найден или роль неверна");
  }


  [HttpGet("{eventId}/tasks/step2")]
  public async Task<IActionResult> GetStep2Tasks(int eventId)
  {
    return Ok(await _taskService.GetStep2TasksAsync(eventId));
  }

  [HttpGet("{eventId}/tasks/step3")]
  public async Task<IActionResult> GetStep3Tasks(int eventId)
  {
    return Ok(await _taskService.GetStep3TasksAsync(eventId));
  }

  [HttpPost("create/step2")]
  public Task<IActionResult> CreateStep2([FromBody] TaskCreateRequest request)
  {
    return CreateInternal(request, 2);
  }

  [HttpPost("create/step3")]
  public Task<IActionResult> CreateStep3([FromBody] TaskCreateRequest request)
  {
    return CreateInternal(request, 3);
  }

  [HttpPut("tasks-board/update-status")]
  public async Task<IActionResult> UpdateStatus([FromBody] UpdateTaskStatusDto dto)
  {
    var result = await _taskService.UpdateTaskStatusAsync(dto);
    return Ok(result);
  }

  [HttpGet("{eventId}/tasks")]
  public async Task<IActionResult> GetTasks(int eventId)
  {
    var tasks = await _taskService.GetTasksByEventAsync(eventId);

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

  [HttpDelete("create/step3")]
  [HttpDelete("tasks-board")]
  public async Task<IActionResult> DeleteTask([FromBody] TaskDeleteRequest request)
  {
    if (request == null || request.TaskId == 0)
      return BadRequest("ID задачи не указан");

    var success = await _taskService.DeleteTaskAsync(request.TaskId);
    return success ? Ok() : NotFound("Задача не найдена");
  }

  [HttpPost("create/step2/search")]
  public async Task<IActionResult> SearchPerformers([FromBody] PerformerSearchDto dto)
  {
    if (string.IsNullOrWhiteSpace(dto.Specialization))
      return BadRequest("Укажите специализацию для поиска");

    try
    {
      var result = await _taskService.SearchPerformersAsync(dto);
      return Ok(result.Select(u => new
      {
        u.Id,
        u.FullName,
        Login = u.UserName,
        ServiceName = u.Specialization,
        u.City,
        u.Description
      }));
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("{tenderId}/accept")]
  public async Task<IActionResult> AcceptTaskFromTender(int tenderId)
  {
    var employeeId = GetUserId();

    try
    {
      await _taskService.CreateTaskFromTenderAsync(tenderId, employeeId);
      return Ok();
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
  }
}

