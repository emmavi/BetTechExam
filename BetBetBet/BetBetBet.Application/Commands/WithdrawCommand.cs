using BetBetBet.Domain.ValueObjects;

namespace BetBetBet.Application.Commands;

public sealed record WithdrawCommand(Money Amount) : ICommand;
