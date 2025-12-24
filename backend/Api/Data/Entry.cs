using Mess.Data;

namespace Mess.Api.Data;

public class ApiEntry : Entry
{
    public string MyPrice { get; set; }
    public int MyPersonCount { get; }
    public string PricePerPerson { get; set; }
    public string TotalPrice { get; set; }
}