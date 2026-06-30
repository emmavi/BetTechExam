using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Domain.Common;
using BetBetBet.Domain.Errors;
using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Presentation.Parsing;

public sealed class BetCommandParser : ICommandParser
{
    public string Keyword => "bet";

    public Result<ICommand> Parse(string[] args)
    {
        if (args.Length == 0)
            return InputErrors.InvalidFormat;

        if (!decimal.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
            return InputErrors.InvalidFormat;

        if (amount <= 0)
            return InputErrors.InvalidFormat;

        var moneyResult = Money.Create(amount);
        if (moneyResult.IsFailure)
            return moneyResult.Error!;

        return new BetCommand(moneyResult.Value!);
    }
}
