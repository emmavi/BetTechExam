using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;

namespace BetBetBet.Presentation.Commands;

public sealed class ExitCommandExecutor : ICommandExecutor
{
    private readonly IConsole _console;

    public ExitCommandExecutor(IConsole console)
    {
        _console = console;
    }

    public Type CommandType => typeof(ExitCommand);

    public ExecutionResult Execute(ICommand command, Wallet currentWallet)
    {
        _console.WriteLine("Thank you for playing! Hope to see you again soon.");
        return new ExecutionResult(null, true);
    }
}
