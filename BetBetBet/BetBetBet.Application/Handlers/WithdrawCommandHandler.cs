using BetBetBet.Application.Commands;
using BetBetBet.Domain.Common;
using BetBetBet.Domain.Entities;

namespace BetBetBet.Application.Handlers;

public sealed class WithdrawCommandHandler
{
    public Result<Wallet> Handle(Wallet wallet, WithdrawCommand command)
    {
        return wallet.Withdraw(command.Amount);
    }
}
