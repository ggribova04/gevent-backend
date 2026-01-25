using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/events")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IHttpContextAccessor _httpContext;

    public EventController(IEventService eventService, IHttpContextAccessor httpContext)
    {
        _eventService = eventService;
        _httpContext = httpContext;
    }

    private int GetUserId() => int.Parse(_httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMyEvents() =>
        Ok(await _eventService.GetEventsByUserAsync(GetUserId()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var result = await _eventService.GetEventByIdAsync(id, GetUserId());
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("create/step1")]
    public async Task<IActionResult> Create([FromBody] EventDto dto)
    {
        var userId = GetUserId();
        dto.IdOrganizer = userId;

        // Проверяем, есть ли ID мероприятия в сессии
        var eventId = HttpContext.Session.GetInt32("CurrentEventId");

        EventDto result;

        if (eventId.HasValue)
        {
            // Обновление существующего мероприятия
            dto.Id = eventId.Value;
            var updated = await _eventService.UpdateEventAsync(dto);

            if (!updated)
                return NotFound("Не удалось обновить мероприятие");

            result = dto;
        }
        else
        {
            // Создание нового мероприятия
            result = await _eventService.CreateEventAsync(dto);
            HttpContext.Session.SetInt32("CurrentEventId", result.Id);
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EventDto dto)
    {
        dto.IdOrganizer = GetUserId();
        var updated = await _eventService.UpdateEventAsync(dto);
        return updated ? Ok(dto) : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        await _eventService.DeleteEventAsync(id, GetUserId()) ? Ok() : Forbid();

    [HttpGet("create/step1")]
    public async Task<IActionResult> GetCurrentEvent()
    {
        var eventId = HttpContext.Session.GetInt32("CurrentEventId");

        if (!eventId.HasValue)
            return NoContent();

        var eventDto = await _eventService.GetEventByIdAsync(eventId.Value, GetUserId());

        return eventDto == null ? NotFound() : Ok(eventDto);
    }

    [HttpPost("reset")]
    public IActionResult ResetCurrentEvent()
    {
        HttpContext.Session.Remove("CurrentEventId");
        return Ok(new { message = "Сессия сброшена" });
    }
}