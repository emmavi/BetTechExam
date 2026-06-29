using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Application.Commands;

public sealed record DepositCommand(Money Amount) : ICommand;
