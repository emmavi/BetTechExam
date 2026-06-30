using BetBetBet.Application.Commands;
using BetBetBet.Application.Results;
using BetBetBet.Domain.Common;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Errors;
using BetBetBet.Domain.Services;

namespace BetBetBet.Application.Handlers;

public sealed class BetCommandHandler
{
    private readonly IGameEngine _gameEngine;

    public BetCommandHandler(IGameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public Result<BetResult> Handle(Wallet wallet, BetCommand command)
    {
        if (!BetRules.IsValid(command.Amount))
            return BetErrors.OutOfRange(command.Amount, wallet.Balance);

        if (command.Amount > wallet.Balance)
            return BetErrors.InsufficientFunds(wallet.Balance);

        var outcome = _gameEngine.Play(command.Amount);
        var walletResult = wallet.ApplyBetOutcome(outcome);

        if (walletResult.IsFailure)
            return walletResult.Error!;

        var newWallet = walletResult.Value!;
        return new BetResult(outcome.Bet, outcome.Win, newWallet.Balance);
    }
}
