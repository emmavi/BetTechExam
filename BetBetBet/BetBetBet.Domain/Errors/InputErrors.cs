using BetBetBet.Domain.Common;

namespace BetBetBet.Domain.Errors;

public static class InputErrors
{
    public static readonly Error UnknownCommand =
        new("Input.UnknownCommand", "Unknown command.");

    public static readonly Error InvalidFormat =
        new("Input.InvalidFormat", "Could not parse amount.");

    public static readonly Error DepositAmountMustBePositive =
        new("Input.DepositAmountMustBePositive", "Deposit amount must be positive.");
}
