using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
using BetBetBet.Application.Results;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Services;

namespace BetBetBet.Presentation.Commands;

public sealed class BetCommandExecutor : ICommandExecutor
{
    private readonly IConsole _console;
    private readonly IGameEngine _gameEngine;

    public BetCommandExecutor(IConsole console, IGameEngine gameEngine)
    {
        _console = console;
        _gameEngine = gameEngine;
    }

    public Type CommandType => typeof(BetCommand);

    public ExecutionResult Execute(ICommand command, Wallet currentWallet)
    {
        var betCommand = (BetCommand)command;
        var handler = new BetCommandHandler(_gameEngine);
        var result = handler.Handle(currentWallet, betCommand);

        if (result.IsSuccess)
        {
            var betResult = result.Value!;
            var updatedWallet = new Wallet(betResult.NewBalance);

            if (betResult.IsWin)
            {
                _console.WriteLine(BetResultFormatter.FormatWin(betResult));
            }
            else
            {
                _console.WriteLine(BetResultFormatter.FormatLoss(betResult));
            }

            return new ExecutionResult(updatedWallet, false);
        }

        _console.WriteLine(result.Error!.Message);
        return new ExecutionResult(null, false);
    }
}
