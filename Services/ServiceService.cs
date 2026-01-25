using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

public class ServiceService : IServiceService
{
    private readonly ApplicationDbContext _context;

    public ServiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PerformerDto> AddByLoginAsync(AddPerformerByLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.Login);
        if (user == null)
            throw new ArgumentException("Пользователь не найден");

        var exists = await _context.Services
           .AnyAsync(s => s.EventId == dto.IdEvent && s.SupplierId == user.Id);

        if (exists)
            throw new ArgumentException("Исполнитель уже добавлен в мероприятие");

        var service = new Service
        {
            Title = user.Specialization,
            SupplierId = user.Id,
            EventId = dto.IdEvent,
            IdStatus = 0
        };
        _context.Services.Add(service);

        var existing = await _context.Organizations.FirstOrDefaultAsync(o =>
            o.UserId == user.Id && o.EventId == dto.IdEvent);

        if (existing == null)
        {
            existing = new Organization
            {
                UserId = user.Id,
                EventId = dto.IdEvent,
                IdRole = 2 // исполнитель
            };
            _context.Organizations.Add(existing);
        }
        else
        {
            existing.IdRole = 2; // если по ошибке роль была другой
        }

        await _context.SaveChangesAsync();

        // Найдём статус "Не выполнено" по умолчанию
        var defaultStatus = await _context.TaskStatuses.FirstOrDefaultAsync(s => s.Name == "Не выполнено");

        return new PerformerDto
        {
            Id = service.Id,
            FullName = user.FullName,
            Login = user.UserName,
            Specialization = user.Specialization ?? "—",
            Status = defaultStatus?.Name ?? "Не выполнено",
            StatusId = service.IdStatus
        };
    }

    public async Task<List<User>> SearchPerformersAsync(PerformerSearchDto dto)
    {
        var query = _context.Users.AsQueryable();

        query = query.Where(u => u.IdRole == 2);

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

    public async Task<PerformerDto> AddFoundPerformerAsync(PerformerChoiceDto dto)
    {
        var user = await _context.Users.FindAsync(dto.IdUser);
        if (user == null)
            throw new ArgumentException("Пользователь не найден");

        var exists = await _context.Services
           .AnyAsync(s => s.EventId == dto.IdEvent && s.SupplierId == user.Id);

        if (exists)
            throw new ArgumentException("Исполнитель уже добавлен в мероприятие");

        var service = new Service
        {
            Title = user.Specialization,
            SupplierId = user.Id,
            EventId = dto.IdEvent,
            IdStatus = 0
        };
        _context.Services.Add(service);

        var existing = await _context.Organizations.FirstOrDefaultAsync(o =>
            o.UserId == user.Id && o.EventId == dto.IdEvent);

        if (existing == null)
        {
            existing = new Organization
            {
                UserId = user.Id,
                EventId = dto.IdEvent,
                IdRole = 2 // исполнитель
            };
            _context.Organizations.Add(existing);
        }

        await _context.SaveChangesAsync();

        // Найдём статус "Не выполнено" по умолчанию
        var defaultStatus = await _context.TaskStatuses.FirstOrDefaultAsync(s => s.Name == "Не выполнено");

        return new PerformerDto
        {
            Id = service.Id,
            FullName = user.FullName,
            Login = user.UserName,
            Specialization = user.Specialization ?? "—",
            Status = defaultStatus?.Name ?? "Не выполнено",
            StatusId = service.IdStatus
        };
    }

    public async Task<List<PerformerDto>> GetPerformersByEventIdAsync(int eventId)
    {
        var result = await _context.Services
            .Where(s => s.EventId == eventId)
            .Include(s => s.Supplier)
            .Include(s => s.Status)
            .Select(s => new PerformerDto
            {
                Id = s.Id,
                FullName = s.Supplier.FullName,
                Login = s.Supplier.UserName,
                Specialization = s.Title,
                Status = s.Status.Name,
                StatusId = s.IdStatus
            })
            .ToListAsync();

        return result;
    }

    public async Task<PerformerStatusUpdateResultDto> UpdatePerformerStatusAsync(UpdatePerformerStatusDto dto)
    {
        var service = await _context.Services.FindAsync(dto.PerformerId);
        if (service == null)
            throw new ArgumentException("Исполнитель не найден");

        service.IdStatus = dto.NewStatusId;
        await _context.SaveChangesAsync();

        return new PerformerStatusUpdateResultDto
        {
            PerformerId = service.Id,
            NewStatusId = service.IdStatus,
            StatusName = service.IdStatus == 1 ? "Выполнено" : "Не выполнено"
        };
    }

    public async Task<List<PerformerDto>> GetAllUserPerformersAsync(int userId, int userRoleId)
    {
        var result = new List<PerformerDto>();

        if (userRoleId == 1)
        {
            // Администратор: получить все задачи, где он — организатор
            var eventIds = await _context.Events
                .Where(e => e.OrganizerId == userId)
                .Select(e => e.Id)
                .ToListAsync();

            var performerTasks = await _context.Services
                .Where(s => eventIds.Contains(s.EventId))
                .Include(s => s.Supplier)
                .Include(s => s.Status)
                .Include(s => s.Event)
                .ToListAsync();

            result.AddRange(performerTasks.Select(s => new PerformerDto
            {
                Id = s.Id,
                FullName = s.Supplier.FullName + " - " + s.Event.Title,
                Specialization = s.Supplier.Specialization, // Специализация как описание задачи
                StatusId = s.IdStatus,
                Status = s.Status.Name,
                Date = s.Event.Date
            }));
        }
        else if (userRoleId == 2)
        {
            // Исполнитель: задачи из Service, где SupplierId = userId
            var performerTasks = await _context.Services
                .Where(s => s.SupplierId == userId)
                .Include(s => s.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(s => s.Supplier)
                .Include(s => s.Status)
                .ToListAsync();

            result.AddRange(performerTasks.Select(s => new PerformerDto
            {
                Id = s.Id,
                FullName = s.Event.Title,
                Specialization = "Администратор назначивший задание: " + s.Event.Organizer.FullName,
                StatusId = s.IdStatus,
                Status = s.Status.Name,
                Date = s.Event.Date
            }));
        }
        
        return result;
    }
}
