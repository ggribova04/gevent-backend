namespace gevent.Database.Entities
{
  public class UserTenderPreference
  {
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int TenderId { get; set; }
    public Tender Tender { get; set; }

    public bool IsHidden { get; set; } = false;

    public bool IsRejected { get; set; } = false;
  }
}
