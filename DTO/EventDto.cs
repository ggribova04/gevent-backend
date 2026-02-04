using gevent.Database.Enums;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time {  get; set; }
    public EventState Status { get; set; }
    public int? OrganizerId {  get; set; }
}
