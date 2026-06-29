using BetBetBet.Application.Commands;
using BetBetBet.Domain.Common;
using BetBetBet.Domain.Entities;

namespace BetBetBet.Application.Handlers;

public sealed class DepositCommandHandler
{
    public Result<Wallet> Handle(Wallet wallet, DepositCommand command)
    {
        return wallet.Deposit(command.Amount);
    }
}
