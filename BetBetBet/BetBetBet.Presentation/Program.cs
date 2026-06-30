using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Services;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Infra;
using BetBetBet.Presentation.Commands;
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

        var randomProvider = new SystemRandomProvider();
        var gameEngine = new SlotGameEngine(randomProvider);

        var parserRegistry = new CommandParserRegistry([
            new ExitCommandParser(),
            new DepositCommandParser(),
            new WithdrawCommandParser(),
            new BetCommandParser()
        ]);

        var executorRegistry = new CommandExecutorRegistry([
            new ExitCommandExecutor(console),
            new DepositCommandExecutor(console),
            new WithdrawCommandExecutor(console),
            new BetCommandExecutor(console, gameEngine)
        ]);

        while (true)
        {
            console.WriteLine("Please enter a command:");
            var input = console.ReadLine();

            var parseResult = parserRegistry.Parse(input);

            if (parseResult.IsFailure)
            {
                console.WriteLine(parseResult.Error!.Message);
                continue;
            }

            if (executorRegistry.TryGetExecutor(parseResult.Value, out var executor))
            {
                var result = executor!.Execute(parseResult.Value, wallet);

                if (result.UpdatedWallet is not null)
                {
                    wallet = result.UpdatedWallet;
                }

                if (result.ShouldExit)
                {
                    break;
                }
            }
            else
            {
                console.WriteLine("Unknown command.");
            }
        }
    }
}
