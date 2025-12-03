using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mess.Data;

public class Entry
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }
    public string Name { get; set; }
    public Group Group { get; set; }
    public List<Member> Participants { get; set; } = new ();
}