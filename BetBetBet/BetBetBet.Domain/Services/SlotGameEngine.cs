using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Services;

public sealed class SlotGameEngine : IGameEngine
{
    private readonly IRandomProvider _randomProvider;

    public SlotGameEngine(IRandomProvider randomProvider)
    {
        _randomProvider = randomProvider;
    }

    public BetOutcome Play(Money bet)
    {
        var roll = _randomProvider.NextDecimal();

        if (roll < 0.5m)
            return new BetOutcome(bet, Money.Create(0).Value!);

        if (roll < 0.9m)
        {
            var multiplier = _randomProvider.NextDecimal() * 2m;
            if (multiplier <= 0)
                multiplier = 0.01m;

            var winAmount = bet.Amount * multiplier;
            return new BetOutcome(bet, Money.Create(winAmount).Value!);
        }

        var bigMultiplier = 2m + _randomProvider.NextDecimal() * 8m;
        if (bigMultiplier <= 2m)
            bigMultiplier = 2.01m;

        var bigWinAmount = bet.Amount * bigMultiplier;
        return new BetOutcome(bet, Money.Create(bigWinAmount).Value!);
    }
}
