using System.Globalization;
using BetBetBet.Domain.Common;
using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Errors;

public static class BetErrors
{
    public static Error OutOfRange(Money attempted, Money balance) =>
        new("Bet.OutOfRange",
            $"Bet must be between $1.00 and $10.00. You tried to bet ${attempted.Amount.ToString("F2", CultureInfo.InvariantCulture)}.");

    public static Error InsufficientFunds(Money balance) =>
        new("Bet.InsufficientFunds",
            $"Insufficient funds for bet. Your current balance is: ${balance.Amount.ToString("F2", CultureInfo.InvariantCulture)}");
}
