using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
using BetBetBet.Domain.Entities;

namespace BetBetBet.Presentation.Commands;

public sealed class DepositCommandExecutor : ICommandExecutor
{
    private readonly IConsole _console;

    public DepositCommandExecutor(IConsole console)
    {
        _console = console;
    }

    public Type CommandType => typeof(DepositCommand);

    public ExecutionResult Execute(ICommand command, Wallet currentWallet)
    {
        var depositCommand = (DepositCommand)command;
        var handler = new DepositCommandHandler();
        var result = handler.Handle(currentWallet, depositCommand);

        if (result.IsSuccess)
        {
            var updatedWallet = result.Value!;
            _console.WriteLine($"Your deposit of ${depositCommand.Amount.Amount} was successful. Your current balance is: ${updatedWallet.Balance.Amount}");
            return new ExecutionResult(updatedWallet, false);
        }

        _console.WriteLine(result.Error!.Message);
        return new ExecutionResult(null, false);
    }
}
