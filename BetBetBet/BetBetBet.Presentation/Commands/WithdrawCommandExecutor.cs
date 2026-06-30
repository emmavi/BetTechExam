using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
using BetBetBet.Domain.Entities;

namespace BetBetBet.Presentation.Commands;

public sealed class WithdrawCommandExecutor : ICommandExecutor
{
    private readonly IConsole _console;

    public WithdrawCommandExecutor(IConsole console)
    {
        _console = console;
    }

    public Type CommandType => typeof(WithdrawCommand);

    public ExecutionResult Execute(ICommand command, Wallet currentWallet)
    {
        var withdrawCommand = (WithdrawCommand)command;
        var handler = new WithdrawCommandHandler();
        var result = handler.Handle(currentWallet, withdrawCommand);

        if (result.IsSuccess)
        {
            var updatedWallet = result.Value!;
            _console.WriteLine($"Your withdrawal of ${withdrawCommand.Amount.Amount.ToString("F2", CultureInfo.InvariantCulture)} was successful. Your current balance is: ${updatedWallet.Balance.Amount.ToString("F2", CultureInfo.InvariantCulture)}");
            return new ExecutionResult(updatedWallet, false);
        }

        _console.WriteLine(result.Error!.Message);
        return new ExecutionResult(null, false);
    }
}
