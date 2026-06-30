using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;

namespace BetBetBet.Presentation.Commands;

public interface ICommandExecutor
{
    Type CommandType { get; }
    ExecutionResult Execute(ICommand command, Wallet currentWallet);
}
