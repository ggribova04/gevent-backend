using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EventService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EventDto>> GetEventsByUserAsync(int userId)
    {
        var eventIds = await _context.Organizations
            .Where(o => o.UserId == userId)
            .Select(o => o.EventId)
            .ToListAsync();

        var events = await _context.Events
            .Where(e => eventIds.Contains(e.Id))
            .ToListAsync();

        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

    public async Task<EventDto?> GetEventByIdAsync(int id, int userId)
    {
        var isParticipant = await _context.Organizations.AnyAsync(o => o.UserId == userId && o.EventId == id);
        if (!isParticipant) return null;

        var ev = await _context.Events.FindAsync(id);
        return _mapper.Map<EventDto>(ev);
    }

    public async Task<EventDto> CreateEventAsync(EventDto dto)
    {
        var entity = _mapper.Map<Event>(dto);
        
        _context.Events.Add(entity);
        await _context.SaveChangesAsync();

        if (dto.OrganizerId == null)
            throw new ArgumentException("IdOrganizer не может быть null");

        _context.Organizations.Add(new Organization
        {
            UserId = dto.OrganizerId.Value,
            EventId = entity.Id,
            IdRole = 1
        });

        await _context.SaveChangesAsync();
        return _mapper.Map<EventDto>(entity);
    }


    public async Task<bool> UpdateEventAsync(EventDto dto)
    {
        var entity = await _context.Events.FindAsync(dto.Id);
        if (entity == null) return false;

        _mapper.Map(dto, entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEventAsync(int id, int userId)
    {
        var isOrganizer = await _context.Organizations.AnyAsync(o =>
            o.UserId == userId && o.EventId == id && o.IdRole == 1);

        if (!isOrganizer) return false;

        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return false;

        // Удаляем связанные записи
        var tasks = _context.Tasks.Where(t => t.EventId == id);
        var guestEvents = _context.EventGuests.Where(g => g.EventId == id);
        var organizations = _context.Organizations.Where(o => o.EventId == id);

        _context.Tasks.RemoveRange(await tasks.ToListAsync());
        _context.EventGuests.RemoveRange(await guestEvents.ToListAsync());
        _context.Organizations.RemoveRange(await organizations.ToListAsync());

        // Удаляем само событие
        _context.Events.Remove(ev);

        await _context.SaveChangesAsync();
        return true;
    }

}
