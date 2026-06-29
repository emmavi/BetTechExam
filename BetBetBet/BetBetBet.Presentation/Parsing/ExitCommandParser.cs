using BetBetBet.Application.Commands;
using BetBetBet.Domain.Common;

namespace BetBetBet.Presentation.Parsing;

public sealed class ExitCommandParser : ICommandParser
{
    public string Keyword => "exit";

    public Result<ICommand> Parse(string[] args)
    {
        if (args.Length > 0)
            return new Error("ExitCommand.UnexpectedArguments", "exit command does not accept arguments.");

        return new ExitCommand();
    }
}
