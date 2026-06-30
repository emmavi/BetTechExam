using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Domain.Services;

public sealed record BetOutcome(Money Bet, Money Win);
