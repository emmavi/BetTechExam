using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Services;

public interface IGameEngine
{
    BetOutcome Play(Money bet);
}
