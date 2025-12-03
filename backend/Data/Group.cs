using System.ComponentModel.DataAnnotations.Schema;

namespace Mess.Data;

public class Group
{    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }
    public string Name { get; set; } = "Unknown";
    public List<Member> Membery { get; set; } = new();
}