using Microsoft.EntityFrameworkCore;
using gevent.Database.Enums;
using Microsoft.AspNetCore.SignalR;

public class TenderService : ITenderService
{
  private readonly ApplicationDbContext _context;
  private readonly ITaskService _taskService;
  private readonly IHubContext<TendersHub> _hubContext;

  public TenderService(ApplicationDbContext context, ITaskService taskService, IHubContext<TendersHub> hubContext)
  {
    _context = context;
    _taskService = taskService;
    _hubContext = hubContext;
  }

  // 1️⃣ Создание тендера
  public async Task<int> CreateTenderAsync(CreateTenderRequest request)
  {
    var tender = new Tender
    {
      EventId = request.EventId,
      Title = request.Title,
      City = request.City,
      ServiceName = request.ServiceName,
      Deadline = request.Deadline,
      Contacts = request.Contacts,
      Comment = request.Comment,
      Status = TenderState.Open
    };

    _context.Tenders.Add(tender);
    await _context.SaveChangesAsync();

    await _hubContext.Clients.All.SendAsync("TendersUpdated");

    return tender.Id;
  }

  // 2️⃣ Тендеры для исполнителя (канбан)
  public async Task<IEnumerable<TenderDto>> GetTendersForEmployeeAsync(int employeeId)
  {
    var tenders = await _context.Tenders
        .Include(t => t.Event)
            .ThenInclude(e => e.Organizer)
        .Include(t => t.Responses)
        .ToListAsync();

    return tenders.Select(t =>
    {
      var response = t.Responses.FirstOrDefault(r => r.EmployeeId == employeeId);

      return new TenderDto
      {
        Id = t.Id,
        Title = t.Title,
        City = t.City,
        ServiceName = t.ServiceName,
        Deadline = t.Deadline,
        EventId = t.EventId,
        EventTitle = t.Event.Title,
        EventDate = t.Event.Date,
        EventTime = t.Event.Time,
        OrganizerEvent = t.Event.Organizer.FullName,
        Contacts = t.Contacts,
        Comment = t.Comment,
        Status = t.Status,
        ViewStatus = ResolveViewStatus(t, response)
      };
    });
  }

  private static TenderViewStatus ResolveViewStatus(Tender tender, TenderResponse? response)
  {
    if (tender.Status != TenderState.Open)
      return TenderViewStatus.ClosedOrRejected;

    if (response == null)
      return TenderViewStatus.New;

    return response.Status switch
    {
      TenderResponseState.Submitted => TenderViewStatus.WaitingForResult,
      TenderResponseState.Won => TenderViewStatus.Won,
      _ => TenderViewStatus.ClosedOrRejected
    };
  }

  // 3️⃣ Отклик исполнителя
  public async System.Threading.Tasks.Task CreateResponseAsync(int tenderId, int employeeId, CreateTenderResponseRequest request)
  {
    var exists = await _context.TenderResponses
        .AnyAsync(r => r.TenderId == tenderId && r.EmployeeId == employeeId);

    if (exists)
      throw new InvalidOperationException("Отклик уже существует");

    var response = new TenderResponse
    {
      TenderId = tenderId,
      EmployeeId = employeeId,
      CostService = request.CostService,
      Contacts = request.Contacts,
      Comment = request.Comment,
      Status = TenderResponseState.Submitted
    };

    _context.TenderResponses.Add(response);
    await _context.SaveChangesAsync();

    await _hubContext.Clients.All.SendAsync("TendersUpdated");
  }

  // 4️⃣ Отклики на тендер
  public async Task<IEnumerable<TenderResponse>> GetResponsesAsync(int tenderId)
  {
    return await _context.TenderResponses
        .Include(r => r.Employee)
        .Where(r => r.TenderId == tenderId)
        .ToListAsync();
  }

  public async Task<TenderResponseDto?> GetResponseForEmployeeAsync(int tenderId, int employeeId)
  {
    var response = await _context.TenderResponses
      .Include(r => r.Employee)
      .Where(r => r.TenderId == tenderId && r.EmployeeId == employeeId)
      .Select(r => new TenderResponseDto
      {
        Id = r.Id,
        TenderId = r.TenderId,
        EmployeeId = r.EmployeeId,
        EmployeeName = r.Employee.FullName,
        Status = r.Status.ToString(),
        CostService = r.CostService,
        Contacts = r.Contacts,
        Comment = r.Comment
      })
      .FirstOrDefaultAsync();

    return response;
  }

  // Обновление отклика исполнителя
  public async System.Threading.Tasks.Task UpdateResponseAsync(int tenderId, int employeeId, CreateTenderResponseRequest request)
  {
    var response = await _context.TenderResponses
      .FirstOrDefaultAsync(r => r.TenderId == tenderId && r.EmployeeId == employeeId);

    if (response == null)
      throw new InvalidOperationException("Отклик не найден");

    // Обновляем данные
    response.CostService = request.CostService;
    response.Contacts = request.Contacts;
    response.Comment = request.Comment;

    await _context.SaveChangesAsync();

    // Уведомляем всех через SignalR
    await _hubContext.Clients.All.SendAsync("TendersUpdated");
  }

  public async System.Threading.Tasks.Task UpdateTenderAsync(int tenderId, CreateTenderRequest request)
  {
    var tender = await _context.Tenders.FindAsync(tenderId);
    if (tender == null)
      throw new Exception("Tender not found");

    tender.Title = request.Title;
    tender.City = request.City;
    tender.ServiceName = request.ServiceName;
    tender.Deadline = request.Deadline;
    tender.Contacts = request.Contacts;
    tender.Comment = request.Comment;

    await _context.SaveChangesAsync();
  }

  public async System.Threading.Tasks.Task DeleteResponseAsync(int responseId)
  {
    var response = await _context.TenderResponses.FindAsync(responseId);
    if (response != null)
    {
      _context.TenderResponses.Remove(response);
      await _context.SaveChangesAsync();
    }

    await _hubContext.Clients.All.SendAsync("TendersUpdated");
  }

  // 6️⃣ Отметить победителя
  public async System.Threading.Tasks.Task MarkWinnerAsync(int responseId)
  {
    var response = await _context.TenderResponses
        .Include(r => r.Tender)
        .FirstOrDefaultAsync(r => r.Id == responseId);

    if (response == null)
      throw new InvalidOperationException("Отклик не найден");

    var allResponses = await _context.TenderResponses
        .Where(r => r.TenderId == response.TenderId)
        .ToListAsync();

    foreach (var r in allResponses)
      r.Status = r.Id == responseId ? TenderResponseState.Won : TenderResponseState.Lost;

    await _context.SaveChangesAsync();

    await _hubContext.Clients.All.SendAsync("TendersUpdated");
  }

  // 7️⃣ Закрытие тендера
  public async System.Threading.Tasks.Task CloseTenderAsync(int tenderId)
  {
    var tender = await _context.Tenders
        .Include(t => t.Responses)
        .FirstOrDefaultAsync(t => t.Id == tenderId);

    if (tender == null)
      throw new InvalidOperationException("Тендер не найден");

    var winner = tender.Responses.FirstOrDefault(r => r.Status == TenderResponseState.Won);

    if (winner != null)
    {
      tender.Status = TenderState.ClosedWithWinner;
      await _taskService.CreateTaskFromTenderAsync(tender, winner);
    }
    else
    {
      tender.Status = TenderState.ClosedWithoutWinner;
    }

    await _context.SaveChangesAsync();

    await _hubContext.Clients.All.SendAsync("TendersUpdated");
  }
}
