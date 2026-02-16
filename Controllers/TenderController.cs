using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/tenders")]
public class TenderController : ControllerBase
{
  private readonly ITenderService _tenderService;

  public TenderController(ITenderService tenderService)
  {
    _tenderService = tenderService;
  }

  // Создание тендера
  [HttpPost]
  public async Task<ActionResult<int>> CreateTender([FromBody] CreateTenderRequest request)
  {
    var id = await _tenderService.CreateTenderAsync(request);
    return Ok(id);
  }

  // Получение тендеров для исполнителя
  [HttpGet("employee/{employeeId}")]
  public async Task<ActionResult<IEnumerable<TenderDto>>> GetTendersForEmployee(int employeeId)
  {
    var tenders = await _tenderService.GetTendersForEmployeeAsync(employeeId);
    return Ok(tenders);
  }

  // Создание отклика на тендер
  [HttpPost("{tenderId}/response/{employeeId}")]
  public async Task<IActionResult> CreateResponse(int tenderId, int employeeId, [FromBody] CreateTenderResponseRequest request)
  {
    await _tenderService.CreateResponseAsync(tenderId, employeeId, request);
    return Ok();
  }

  // Получение всех откликов на тендер
  [HttpGet("{tenderId}/responses")]
  public async Task<ActionResult<IEnumerable<TenderResponse>>> GetResponses(int tenderId)
  {
    var responses = await _tenderService.GetResponsesAsync(tenderId);
    return Ok(responses);
  }

  // Удаление отклика
  [HttpDelete("response/{responseId}")]
  public async Task<IActionResult> DeleteResponse(int responseId)
  {
    await _tenderService.DeleteResponseAsync(responseId);
    return NoContent();
  }

  // Отметить победителя
  [HttpPost("response/{responseId}/winner")]
  public async Task<IActionResult> MarkWinner(int responseId)
  {
    await _tenderService.MarkWinnerAsync(responseId);
    return Ok();
  }

  // Закрытие тендера
  [HttpPost("{tenderId}/close")]
  public async Task<IActionResult> CloseTender(int tenderId)
  {
    await _tenderService.CloseTenderAsync(tenderId);
    return Ok();
  }
}

