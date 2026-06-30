using System.Globalization;
using BetBetBet.Application.Results;

namespace BetBetBet.Presentation;

public static class BetResultFormatter
{
    public static string FormatWin(BetResult result) =>
        $"Congrats - you won ${result.Win.Amount.ToString("F2", CultureInfo.InvariantCulture)}! Your current balance is: ${result.NewBalance.Amount.ToString("F2", CultureInfo.InvariantCulture)}";

    public static string FormatLoss(BetResult result) =>
        $"No luck this time! Your current balance is: ${result.NewBalance.Amount.ToString("F2", CultureInfo.InvariantCulture)}";
}
