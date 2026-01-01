using Mess.Data;

namespace Mess.Api.Data;

public class ApiEntry : Entry
{
    public string MyPrice { get; set; }
    public int MyPersonCount { get; set; }
    public string PricePerPerson { get; set; }
    public string TotalPrice { get; set; }
    public int TotalPersonCount { get; set; }

    public ApiEntry(Entry e)
    {
        this.Id = e.Id;
        this.Name = e.Name;
        this.GroupId = e.GroupId;
        this.Date = e.Date;
        this.Participants = e.Participants;
    }
}