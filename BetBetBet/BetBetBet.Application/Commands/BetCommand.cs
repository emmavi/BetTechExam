using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Application.Commands;

public sealed record BetCommand(Money Amount) : ICommand;
