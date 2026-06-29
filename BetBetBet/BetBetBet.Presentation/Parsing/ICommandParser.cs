using BetBetBet.Application.Commands;
using BetBetBet.Domain.Common;

namespace BetBetBet.Presentation.Parsing;

public interface ICommandParser
{
    string Keyword { get; }
    Result<ICommand> Parse(string[] args);
}
