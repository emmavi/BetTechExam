using BetBetBet.Application.Commands;

namespace BetBetBet.Presentation.Commands;

public sealed class CommandExecutorRegistry
{
    private readonly Dictionary<Type, ICommandExecutor> _executors;

    public CommandExecutorRegistry(IEnumerable<ICommandExecutor> executors)
    {
        _executors = executors.ToDictionary(e => e.CommandType);
    }

    public bool TryGetExecutor(ICommand command, out ICommandExecutor? executor)
    {
        return _executors.TryGetValue(command.GetType(), out executor);
    }
}
