using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Services;

public static class BetRules
{
    public static readonly Money MinBet = Money.Create(1).Value!;
    public static readonly Money MaxBet = Money.Create(10).Value!;

    public static bool IsValid(Money amount) => amount >= MinBet && amount <= MaxBet;
}
