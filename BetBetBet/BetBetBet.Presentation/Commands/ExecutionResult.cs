using BetBetBet.Domain.Entities;

namespace BetBetBet.Presentation.Commands;

public sealed record ExecutionResult(Wallet? UpdatedWallet, bool ShouldExit);
