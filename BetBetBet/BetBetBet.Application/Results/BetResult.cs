using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Application.Results;

public sealed record BetResult(Money Bet, Money Win, Money NewBalance)
{
    public bool IsWin => Win.Amount > 0;
}
