// Small standalone example; put in a console project to run.
using System;

public class Transaction
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public static class TransactionExtensions
{
    public static string Format(this Transaction transaction)
    {
        return $"{transaction.Amount:N2} {transaction.Currency}";
    }
}

public static class Program
{
    public static void Main()
    {
        var transaction = new Transaction { Amount = 150.50m };
        Console.WriteLine(transaction.Format()); // 150.50 USD
    }
}
