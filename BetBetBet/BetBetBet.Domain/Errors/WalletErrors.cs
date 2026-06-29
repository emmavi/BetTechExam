using System.Globalization;
using BetBetBet.Domain.Common;
using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Errors;

public static class WalletErrors
{
    public static Error InsufficientFunds(Money balance) =>
        new("Wallet.InsufficientFunds",
            $"Insufficient funds for withdrawal. Your current balance is: ${balance.Amount.ToString("F2", CultureInfo.InvariantCulture)}");
}
