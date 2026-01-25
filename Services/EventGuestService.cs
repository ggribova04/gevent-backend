using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class EventGuestService : IEventGuestService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EventGuestService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EventGuestDto>> GetGuestsByEventAsync(int eventId)
    {
        var guests = await _context.EventGuests
            .Where(g => g.EventId == eventId)
            .ToListAsync();

        return _mapper.Map<List<EventGuestDto>>(guests);
    }

    public async Task<bool> AddGuestAsync(int eventId, string guestInfo)
    {
        var guest = new EventGuest
        {
            EventId = eventId,
            GuestInfo = guestInfo
        };

        _context.EventGuests.Add(guest);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGuestAsync(int eventId, int guestId)
    {
        var guest = await _context.EventGuests
            .FirstOrDefaultAsync(g => g.Id == guestId && g.EventId == eventId);

        if (guest == null)
            return false;

        _context.EventGuests.Remove(guest);
        await _context.SaveChangesAsync();
        return true;
    }
}