using System.ComponentModel.DataAnnotations.Schema;

namespace Mess.Data;

public class Group
{    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; } = "Unknown";
    public string CurrencyPostfix { get; set; } = "€";
    public List<Member> Members { get; set; } = new();

    public bool CanBeAccessedBy(User loggedInUser)
    {
        return Members.Any(x => x.User?.Id == loggedInUser.Id);
    }

    public bool CanBeEditedBy(User loggedInUser)
    {
        return CanBeAccessedBy(loggedInUser);
    }
}