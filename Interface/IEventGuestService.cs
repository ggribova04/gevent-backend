public interface IEventGuestService
{
    Task<List<EventGuestDto>> GetGuestsByEventAsync(int eventId);
    Task<bool> AddGuestAsync(int eventId, string guestInfo);
    Task<bool> DeleteGuestAsync(int eventId, int guestId);
}