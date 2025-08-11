using System;
using System.Collections.Generic;

// Record for Transaction - immutable by default
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface for transaction processors
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Concrete processor classes
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}.");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Sent {transaction.Amount:C} for {transaction.Category}.");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Transferred {transaction.Amount:C} for {transaction.Category}.");
    }
}

// Base Account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// Sealed SavingsAccount class that inherits from Account
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds.");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction successful. Updated Balance: {Balance:C}");
        }
    }
}

// FinanceApp class to simulate system
public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        // Create a savings account with initial balance of 1000
        var account = new SavingsAccount("ACC-001", 1000m);

        // Create transactions
        var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 700m, "Entertainment");

        // Processors
        var mobileMoneyProcessor = new MobileMoneyProcessor();
        var bankTransferProcessor = new BankTransferProcessor();
        var cryptoWalletProcessor = new CryptoWalletProcessor();

        // Process and apply transactions
        mobileMoneyProcessor.Process(t1);
        account.ApplyTransaction(t1);
        _transactions.Add(t1);

        bankTransferProcessor.Process(t2);
        account.ApplyTransaction(t2);
        _transactions.Add(t2);

        cryptoWalletProcessor.Process(t3);
        account.ApplyTransaction(t3);
        _transactions.Add(t3);
    }

    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
