using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Mess.Data;

public class Balance
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public Guid GroupId { get; set; }
    [JsonIgnore]
    public Group Group { get; set; }
    public Guid PayeeId { get; set; }
    public Member Payee { get; set; }
    public Guid PayerId { get; set; }
    public Member Payer { get; set; }
    
    /// <summary>
    /// Indicates whether payer and payee differ from the requested payer and payee. Therefore needing to negate all transaction amounts being made so they do what's requested 
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    public bool IsSwapped { get; set; }

    /// <summary>
    /// Adds an amount to a balance. IsSwapped is respected
    /// </summary>
    /// <param name="amount">Amount to add</param>
    /// <returns></returns>
    public decimal Add(decimal amount)
    {
        this.Amount += IsSwapped ? amount * -1 : amount;
        return this.Amount;
    }
}