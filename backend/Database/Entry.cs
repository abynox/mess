using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mess.Data;

public class Entry
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Group Group { get; set; }
    public List<Member> Participants { get; set; } = new ();
    public Dictionary<string, decimal> Payees { get; set; } = new();
}