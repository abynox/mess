using System.ComponentModel.DataAnnotations.Schema;
using Mess.Api.Data;

namespace Mess.Data;

public class Member
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid? Id { get; set; }
    public string Name { get; set; } = "Unknown";
    
    /// <summary>
    /// The id of auser associated with a member
    /// </summary>
    public Guid? UserId { get; set; }
    
    /// <summary>
    /// The user associated with a member
    /// </summary>
    public User? User { get; set; }
    
    public Member() { }

    public Member(User user)
    {
        Name = user.Name;
        UserId = user.Id;
    }
    
    public Member(ApiMemberRequest  request)
    {
        Name = request.Name;
    }
}