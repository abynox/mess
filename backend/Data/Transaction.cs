using System.ComponentModel.DataAnnotations.Schema;

namespace Mess.Data;

public class Transaction(int amount)
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Uuid { get; set; }
    public int Amount { get; set; } = amount;
    public required Group Group { get; set; }
    public required Member Payee { get; set; }
    public required Member Payer { get; set; }
}