using BetBetBet.Domain.Common;
using BetBetBet.Domain.Errors;
using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Entities;

public sealed class Wallet
{
    public Money Balance { get; }

    public Wallet(Money balance)
    {
        Balance = balance;
    }

    public Result<Wallet> Deposit(Money amount)
    {
        if (amount <= Money.Create(0).Value!)
            return InputErrors.DepositAmountMustBePositive;

        return new Wallet(Balance + amount);
    }
}
