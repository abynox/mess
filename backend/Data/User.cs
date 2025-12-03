using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Mess.Data;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public string OidcId { get; set; }
}