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

  [HttpGet("{tenderId}")]
  public async Task<ActionResult<TenderDto?>> GetTender(int tenderId)
  {
    var tender = await _tenderService.GetTenderByIdAsync(tenderId);
    if (tender == null)
      return NotFound();

    return Ok(tender);
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
  public async Task<ActionResult<IEnumerable<TenderResponseDto>>> GetResponses(int tenderId)
  {
    var responses = await _tenderService.GetResponsesAsync(tenderId);
    return Ok(responses);
  }

  [HttpGet("{tenderId}/response/{employeeId}")]
  public async Task<ActionResult<TenderResponseDto?>> GetResponseForEmployee(int tenderId, int employeeId)
  {
    var response = await _tenderService.GetResponseForEmployeeAsync(tenderId, employeeId);
    if (response == null)
      return NotFound();

    return Ok(response);
  }

  [HttpPut("{tenderId}/response/{employeeId}")]
  public async Task<IActionResult> UpdateResponse(int tenderId, int employeeId, [FromBody] CreateTenderResponseRequest request)
  {
    await _tenderService.UpdateResponseAsync(tenderId, employeeId, request);
    return Ok();
  }

  [HttpPut("{tenderId}")]
  public async Task<IActionResult> UpdateTender(
    int tenderId,
    [FromBody] CreateTenderRequest request)
  {
    await _tenderService.UpdateTenderAsync(tenderId, request);
    return Ok();
  }

  // Удаление отклика
  [HttpDelete("response/{responseId}")]
  public async Task<IActionResult> DeleteResponse(int responseId)
  {
    await _tenderService.DeleteResponseAsync(responseId);
    return NoContent();
  }

  [HttpPost("{tenderId}/close")]
  public async Task<IActionResult> CloseTender(
    int tenderId,
    [FromBody] CloseTenderRequest request
  )
  {
    await _tenderService.CloseTenderAsync(
      tenderId,
      request.WinnerResponseId
    );

    return Ok();
  }

  // Удаление тендера
  [HttpDelete("{tenderId}")]
  public async Task<IActionResult> DeleteTender(int tenderId)
  {
    await _tenderService.DeleteTenderAsync(tenderId);
    return NoContent();
  }

  [HttpPost("response/{responseId}/select")]
  public async Task<IActionResult> ToggleResponseSelected(int responseId)
  {
    await _tenderService.ToggleResponseSelectedAsync(responseId);
    return Ok();
  }
}

