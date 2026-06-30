using BetBetBet.Domain.Services;

namespace BetBetBet.Infra;

public sealed class SystemRandomProvider : IRandomProvider
{
    private readonly global::System.Random _random = new();

    public decimal NextDecimal() => (decimal)_random.NextDouble();
}
