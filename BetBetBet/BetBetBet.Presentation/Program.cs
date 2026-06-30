using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
using BetBetBet.Application.Results;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Services;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Infra;
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

        var registry = new CommandParserRegistry([
            new ExitCommandParser(),
            new DepositCommandParser(),
            new WithdrawCommandParser(),
            new BetCommandParser()
        ]);

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

            if (parseResult.Value is WithdrawCommand withdrawCommand)
            {
                var handler = new WithdrawCommandHandler();
                var result = handler.Handle(wallet, withdrawCommand);

                if (result.IsSuccess)
                {
                    wallet = result.Value!;
                    console.WriteLine($"Your withdrawal of ${withdrawCommand.Amount.Amount.ToString("F2", CultureInfo.InvariantCulture)} was successful. Your current balance is: ${wallet.Balance.Amount.ToString("F2", CultureInfo.InvariantCulture)}");
                }
                else
                {
                    console.WriteLine(result.Error!.Message);
                }

                continue;
            }

            if (parseResult.Value is BetCommand betCommand)
            {
                var handler = new BetCommandHandler(gameEngine);
                var result = handler.Handle(wallet, betCommand);

                if (result.IsSuccess)
                {
                    var betResult = result.Value!;
                    wallet = new Wallet(betResult.NewBalance);

                    if (betResult.IsWin)
                    {
                        console.WriteLine(BetResultFormatter.FormatWin(betResult));
                    }
                    else
                    {
                        console.WriteLine(BetResultFormatter.FormatLoss(betResult));
                    }
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
