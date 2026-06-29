using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
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

        var registry = new CommandParserRegistry([new ExitCommandParser(), new DepositCommandParser()]);

        while (true)
        {
            console.WriteLine("Please enter a command:");
            var input = console.ReadLine();

            var parseResult = registry.Parse(input);

            if (parseResult.IsFailure)
            {
                console.WriteLine(parseResult.Error!.Message);
                continue;
            }

            if (parseResult.Value is ExitCommand)
            {
                console.WriteLine("Thank you for playing! Hope to see you again soon.");
                break;
            }

            if (parseResult.Value is DepositCommand depositCommand)
            {
                var handler = new DepositCommandHandler();
                var result = handler.Handle(wallet, depositCommand);

                if (result.IsSuccess)
                {
                    wallet = result.Value!;
                    console.WriteLine($"Your deposit of ${depositCommand.Amount.Amount} was successful. Your current balance is: ${wallet.Balance.Amount}");
                }
                else
                {
                    console.WriteLine(result.Error!.Message);
                }

                continue;
            }

            console.WriteLine("Unknown command.");
        }
    }
}
