public class AddPerformerByLoginDto
{
    public string Login { get; set; } = null!;
    public int EventId { get; set; }
}

public class PerformerSearchDto
{
    public string Specialization { get; set; } = null!;
    public string City { get; set; } = null!;
}

public class PerformerChoiceDto
{
    public int UserId { get; set; }
    public int EventId { get; set; }
}

public class PerformerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public string Status { get; set; }
    public DateOnly Date { get; set; }
    public string DateString => Date.ToString("dd.MM.yyyy");
}

public class UpdatePerformerStatusDto
{
    public int PerformerId { get; set; }
    public int NewStatusId { get; set; }
}

public class PerformerStatusUpdateResultDto
{
    public int PerformerId { get; set; }
    public int NewStatusId { get; set; }
    public string StatusName { get; set; }
}

