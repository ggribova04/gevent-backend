public interface IEventService
{
    Task<IEnumerable<EventDto>> GetEventsByUserAsync(int userId);
    Task<EventDto?> GetEventByIdAsync(int id, int userId);
    Task<EventDto> CreateEventAsync(EventDto dto);
    Task<bool> UpdateEventAsync(EventDto dto);
    Task<bool> DeleteEventAsync(int id, int userId);
}