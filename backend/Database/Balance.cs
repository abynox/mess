using System.ComponentModel.DataAnnotations.Schema;

namespace Mess.Data;

public class Balance
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public required Group Group { get; set; }
    public required Member Payee { get; set; }
    public required Member Payer { get; set; }
}