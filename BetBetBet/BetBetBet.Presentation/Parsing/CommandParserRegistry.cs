using BetBetBet.Application.Commands;
using BetBetBet.Domain.Common;
using BetBetBet.Domain.Errors;

namespace BetBetBet.Presentation.Parsing;

public sealed class CommandParserRegistry
{
    private readonly Dictionary<string, ICommandParser> _parsers;

    public CommandParserRegistry(IEnumerable<ICommandParser> parsers)
    {
        _parsers = parsers.ToDictionary(p => p.Keyword, StringComparer.OrdinalIgnoreCase);
    }

    public Result<ICommand> Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return InputErrors.UnknownCommand;

        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return InputErrors.UnknownCommand;

        if (!_parsers.TryGetValue(parts[0], out var parser))
            return InputErrors.UnknownCommand;

        return parser.Parse(parts[1..]);
    }
}
