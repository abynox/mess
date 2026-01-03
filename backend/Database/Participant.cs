using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Mess.Data;

public class Participant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    [NotMapped]
    public string Name => Member.Name;

    public int ExtraParticipants { get; set; }
    // This should be an int cause one cannot pay half a cent irl
    public int PaidAmount { get; set; }
    [NotMapped]
    public string PaidAmountString { get; set; }
    public Guid MemberId { get; set; }
    [JsonIgnore]
    public Member Member { get; set; }
    
    public Participant() {}

    public int GetPersonCount()
    {
        return ExtraParticipants + 1;
    }
}