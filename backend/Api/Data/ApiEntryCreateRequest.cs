namespace Mess.Api.Data;

public class ApiEntryCreateRequest
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public List<ApiEntryCreateRequestParticipant> Participants { get; set; }
}

public class ApiEntryCreateRequestParticipant
{
    public Guid MemberId { get; set; }
    public int ExtraParticipants { get; set; }
    public int PaidAmount { get; set; }
}