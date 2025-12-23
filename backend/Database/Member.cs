using System.ComponentModel.DataAnnotations.Schema;

namespace Mess.Data;

public class Member
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; } = "Unknown";
    public User? AssociatedUser { get; set; }
}