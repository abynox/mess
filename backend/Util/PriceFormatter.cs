using Mess.Data;

namespace Mess.Util;

public class PriceFormatter
{
    public string FormatPrice(decimal amount, Group group)
    {
        return FormatPrice(amount, group.CurrencyPostfix);
    }
    public string FormatPrice(decimal amount, string currencyPostfix, string currencySeperator = ".")
    {
        int roundedPrice = (int)decimal.Floor(amount);
        return (roundedPrice / 100) + currencySeperator + (roundedPrice % 100) + currencyPostfix;
    }
}