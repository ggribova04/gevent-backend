using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("api/events")]
public class ServiceController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    private int? GetCurrentEventId()
    {
        return HttpContext.Session.GetInt32("CurrentEventId");
    }

    [HttpPost("create/step2/add-by-login")]
    public async Task<IActionResult> AddByLogin([FromBody] AddPerformerByLoginDto dto)
    {
        var sessionEventId = GetCurrentEventId();
        var finalEventId = sessionEventId ?? dto.IdEvent;

        if (finalEventId == null || finalEventId == 0)
            return BadRequest("ID мероприятия не найден ни в сессии, ни в параметрах запроса");

        try
        {
            dto.IdEvent = finalEventId;
            var result = await _serviceService.AddByLoginAsync(dto);
            return Ok(new
            {
                result.Id,
                result.FullName,
                result.Login,
                result.Specialization,
                Status = "Не выполнено",
                StatusId = 0
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("create/step2/search")]
    public async Task<IActionResult> SearchPerformers([FromBody] PerformerSearchDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Specialization))
            return BadRequest("Укажите специализацию для поиска");

        try
        {
            var result = await _serviceService.SearchPerformersAsync(dto);
            return Ok(result.Select(u => new
            {
                u.Id,
                u.FullName,
                Login = u.UserName,
                ServiceName = u.Specialization,
                u.City
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("create/step2/add-found-performer")]
    public async Task<IActionResult> AddFoundPerformer([FromBody] PerformerChoiceDto dto)
    {
        var sessionEventId = GetCurrentEventId();
        var finalEventId = sessionEventId ?? dto.IdEvent;

        if (finalEventId == null)
            return BadRequest("ID мероприятия не найден ни в сессии, ни в параметрах запроса");

        try
        {
            dto.IdEvent = finalEventId;
            var result = await _serviceService.AddFoundPerformerAsync(dto);
            return Ok(new
            {
                result.Id,
                result.FullName,
                result.Login,
                ServiceName = result.Specialization,
                Status = "Не выполнено",
                StatusId = 0
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("create/step2/update-status")]
    public async Task<IActionResult> UpdatePerformerStatus([FromBody] UpdatePerformerStatusDto dto)
    {
        if (dto == null)
            return BadRequest("Параметры запроса не заданы");

        try
        {
            var updatedPerformer = await _serviceService.UpdatePerformerStatusAsync(dto);
            return Ok(updatedPerformer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Логирование ошибки здесь, если нужно
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("create/step2/performers")]
    public async Task<IActionResult> GetPerformersForCurrentEvent()
    {
        var eventId = GetCurrentEventId();
        if (eventId == null)
            return BadRequest("ID мероприятия не найден в текущей сессии");

        var result = await _serviceService.GetPerformersByEventIdAsync(eventId.Value);
        return Ok(result.Select(p => new {
            p.Id,
            p.FullName,
            p.Login,
            p.Specialization,
            Status = p.StatusId == 1 ? "Выполнено" : "Не выполнено",
            p.StatusId
        }));
    }
    
    [HttpGet("{eventId}/performers")]
    public async Task<IActionResult> GetPerformersByEvent(int eventId)
    {
        var performers = await _serviceService.GetPerformersByEventIdAsync(eventId);
        return Ok(performers);
    }

    [HttpGet("performers")]
    public async Task<IActionResult> GetMyTasks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleIdClaim = User.FindFirst(ClaimTypes.Role);

        if (userIdClaim == null || roleIdClaim == null)
            return Unauthorized("Claims not found");

        var userId = int.Parse(userIdClaim.Value);
        var roleId = int.Parse(roleIdClaim.Value);

        var performers = await _serviceService.GetAllUserPerformersAsync(userId, roleId);
        return Ok(performers);
    }
}