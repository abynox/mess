using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mess.Data;

public class Entry
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public required Group Group { get; set; }
    public List<Participant> Participants { get; set; } = new ();

    public int GetTotalPersonCount()
    {
        return Participants.Sum(x => x.ExtraParticipants + 1);
    }

    public int GetTotalPrice()
    {
        return (int)Participants.Sum(x => x.PaidAmount);
    }
}