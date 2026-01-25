public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time {  get; set; }
    public int IdStatus { get; set; }
    public int? IdOrganizer {  get; set; }
}