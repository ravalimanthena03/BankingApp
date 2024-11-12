using System;
using System.Collections.Generic;

public class BankingApp
{
    private Dictionary<string, User> users = new Dictionary<string, User>();
    private User loggedInUser;

    public void Start()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nWelcome to Console Banking App");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void Register()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter password: ");
        string password = ReadPassword();

        if (users.ContainsKey(username))
        {
            Console.WriteLine("Username already exists. Please choose another.");
        }
        else
        {
            users[username] = new User(username, password);
            Console.WriteLine("Registration successful!");
        }
    }

    private void Login()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter password: ");
        string password = ReadPassword();

        if (users.ContainsKey(username) && users[username].Login(password))
        {
            loggedInUser = users[username];
            Console.WriteLine("Login successful!");
            UserMenu();
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }


    private void UserMenu()
    {
        bool logout = false;
        while (!logout)
        {
            Console.WriteLine("\n1. Open Account");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. View Statement");
            Console.WriteLine("5. Check Balance");
            Console.WriteLine("6. Calculate Interest");
            Console.WriteLine("7. Logout");
            Console.Write("Select an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    loggedInUser.OpenAccount();
                    break;
                case "2":
                    loggedInUser.MakeDeposit();
                    break;
                case "3":
                    loggedInUser.MakeWithdrawal();
                    break;
                case "4":
                    loggedInUser.ViewStatement();
                    break;
                case "5":
                    loggedInUser.CheckBalance();
                    break;
                case "6":
                    loggedInUser.CalculateInterest();
                    break;
                case "7":
                    logout = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    private string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(intercept: true);

            // Handle Backspace
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
            // Handle Enter key
            else if (key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine(); // Move to the next line after Enter
        return password;
    }
}

public class User
{
    public string Username { get; }
    private string Password { get; }
    private List<Account> Accounts { get; } = new List<Account>();

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public bool Login(string password) => password == Password;

    public void OpenAccount()
    {
        Console.Write("Enter account holder's name: ");
        string holderName = Console.ReadLine();
        Console.Write("Enter account type (1 for Savings, 2 for Checking): ");
        int accountType = int.Parse(Console.ReadLine());

        Console.Write("Enter initial deposit amount: ");
        decimal initialDeposit = decimal.Parse(Console.ReadLine());

        AccountType type = (accountType == 1) ? AccountType.Savings : AccountType.Checking;
        Account newAccount = new Account(holderName, type, initialDeposit);
        Accounts.Add(newAccount);

        Console.WriteLine($"Account created successfully! Account Number: {newAccount.AccountNumber}");
    }

    public void MakeDeposit()
    {
        Account account = SelectAccount();
        if (account == null) return;

        Console.Write("Enter deposit amount: ");
        decimal amount = decimal.Parse(Console.ReadLine());
        account.Deposit(amount);
    }

    public void MakeWithdrawal()
    {
        Account account = SelectAccount();
        if (account == null) return;

        Console.Write("Enter withdrawal amount: ");
        decimal amount = decimal.Parse(Console.ReadLine());
        if (!account.Withdraw(amount))
        {
            Console.WriteLine("Insufficient balance.");
        }
    }

    public void ViewStatement()
    {
        Account account = SelectAccount();
        if (account == null) return;

        account.GenerateStatement();
    }

    public void CheckBalance()
    {
        Account account = SelectAccount();
        if (account == null) return;

        Console.WriteLine($"Current Balance: {account.Balance}");
    }

    public void CalculateInterest()
    {
        foreach (var account in Accounts)
        {
            if (account.Type == AccountType.Savings)
            {
                account.CalculateInterest();
            }
        }
    }

    private Account SelectAccount()
    {
        if (Accounts.Count == 0)
        {
            Console.WriteLine("No accounts available.");
            return null;
        }

        Console.WriteLine("Select an account by number:");
        for (int i = 0; i < Accounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber}");
        }

        int choice = int.Parse(Console.ReadLine());
        return choice > 0 && choice <= Accounts.Count ? Accounts[choice - 1] : null;
    }
}

public class Account
{
    private static int accountCounter = 1000;
    public string AccountNumber { get; }
    public string AccountHolderName { get; }
    public AccountType Type { get; }
    public decimal Balance { get; private set; }
    private List<Transaction> Transactions { get; } = new List<Transaction>();
    private DateTime LastInterestDate;

    public Account(string holderName, AccountType type, decimal initialDeposit)
    {
        AccountNumber = "AC" + (++accountCounter);
        AccountHolderName = holderName;
        Type = type;
        Balance = initialDeposit;
        LastInterestDate = DateTime.Now;
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
        Transactions.Add(new Transaction(amount, TransactionType.Deposit));
        Console.WriteLine($"Deposited {amount}. New balance: {Balance}");
    }

    public bool Withdraw(decimal amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            Transactions.Add(new Transaction(amount, TransactionType.Withdrawal));
            Console.WriteLine($"Withdrew {amount}. New balance: {Balance}");
            return true;
        }
        return false;
    }

    public void GenerateStatement()
    {
        Console.WriteLine($"Statement for Account {AccountNumber}");
        foreach (var transaction in Transactions)
        {
            Console.WriteLine($"{transaction.Date}: {transaction.Type} - {transaction.Amount}");
        }
    }

    public void CalculateInterest()
    {
        if (Type == AccountType.Savings && (DateTime.Now - LastInterestDate).Days >= 30)
        {
            decimal interestRate = 0.03m;  // 3% monthly interest
            decimal interest = Balance * interestRate;
            Balance += interest;
            Transactions.Add(new Transaction(interest, TransactionType.Interest));
            LastInterestDate = DateTime.Now;
            Console.WriteLine($"Interest of {interest} added. New balance: {Balance}");
        }
        else
        {
            Console.WriteLine("Interest calculation is only available for savings accounts once a month.");
        }
    }
}

public class Transaction
{
    public DateTime Date { get; }
    public TransactionType Type { get; }
    public decimal Amount { get; }

    public Transaction(decimal amount, TransactionType type)
    {
        Date = DateTime.Now;
        Type = type;
        Amount = amount;
    }
}

public enum AccountType
{
    Savings,
    Checking
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Interest
}

class Program
{
    static void Main()
    {
        BankingApp app = new BankingApp();
        app.Start();
    }
}
