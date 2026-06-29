using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Presentation.Parsing;

namespace BetBetBet.Presentation;

public static class Program
{
    public static void Main()
    {
        Run(new SystemConsole());
    }

    public static void Run(IConsole console)
    {
        console.WriteLine("Welcome to BetBetBet!");

        var moneyResult = Money.Create(0);
        if (moneyResult.IsFailure)
        {
            console.WriteLine("Failed to initialize wallet.");
            return;
        }

        var wallet = new Wallet(moneyResult.Value);
        console.WriteLine($"Your current balance is: ${wallet.Balance.Amount}");

        var registry = new CommandParserRegistry([new ExitCommandParser()]);

        while (true)
        {
            console.WriteLine("Please enter a command:");
            var input = console.ReadLine();

            var parseResult = registry.Parse(input);

            if (parseResult.IsSuccess && parseResult.Value is ExitCommand)
            {
                console.WriteLine("Thank you for playing! Hope to see you again soon.");
                break;
            }

            console.WriteLine("Unknown command.");
        }
    }
}
