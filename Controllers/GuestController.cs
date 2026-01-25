using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/events")]
public class GuestController : ControllerBase
{
    private readonly IEventGuestService _guestService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GuestController(
        IEventGuestService guestService,
        IHttpContextAccessor httpContextAccessor)
    {
        _guestService = guestService;
        _httpContextAccessor = httpContextAccessor;
    }

    private int? GetCurrentEventId()
    {
        return HttpContext.Session.GetInt32("CurrentEventId");
    }

    [HttpGet("create/step4")]
    public async Task<IActionResult> GetGuests()
    {
        var eventId = GetCurrentEventId();
        if (eventId == null)
        {
            return BadRequest("ID мероприятия не найден в текущей сессии");
        }

        var guests = await _guestService.GetGuestsByEventAsync(eventId.Value);
        return Ok(guests);
    }

    [HttpPost("create/step4")]
    public async Task<IActionResult> AddGuest([FromBody] AddGuestRequest request)
    {
        var sessionEventId = GetCurrentEventId();
        var finalEventId = sessionEventId ?? request.IdEvent;

        if (finalEventId == null || finalEventId == 0)
            return BadRequest("ID мероприятия не найден ни в сессии, ни в параметрах запроса");


        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _guestService.AddGuestAsync(finalEventId, request.GuestInfo);
        return success ? Ok() : BadRequest("Не удалось добавить гостя");
    }

    [HttpDelete("guests/{guestId}")]
    public async Task<IActionResult> DeleteGuest(int guestId)
    {
        // Получаем текущий Id мероприятия, например из сессии или контекста пользователя
        var eventId = GetCurrentEventId();

        if (eventId == null)
        {
            return BadRequest("ID мероприятия не найден в текущей сессии");
        }

        // Вызываем сервис удаления гостя с проверкой успешности
        var success = await _guestService.DeleteGuestAsync(eventId.Value, guestId);

        if (success)
            return Ok();
        else
            return NotFound("Гость не найден");
    }


    [HttpGet("{eventId}/guests")]
    public async Task<IActionResult> GetGuests(int eventId)
    {
        var guests = await _guestService.GetGuestsByEventAsync(eventId);
        return Ok(guests);
    }
}