using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Mess.Data;

public class Participant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    [NotMapped]
    public string Name { get; set; }
    public int ExtraParticipants { get; set; }
    // This should be an int cause one cannot pay half a cent irl
    public int PaidAmount { get; set; }
    [JsonIgnore]
    public Member AssociatedMember { get; set; }
    
    public Participant() {}
}