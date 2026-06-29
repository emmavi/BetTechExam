using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Entities;

public sealed class Wallet
{
    public Money Balance { get; }

    public Wallet(Money balance)
    {
        Balance = balance;
    }
}
