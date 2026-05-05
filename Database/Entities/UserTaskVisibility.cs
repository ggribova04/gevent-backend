namespace gevent.Database.Entities
{
  public class UserTaskVisibility
  {
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int TaskId { get; set; }
    public Task Task { get; set; }

    public bool IsHidden { get; set; } = false;
  }
}
