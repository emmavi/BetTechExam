using BetBetBet.Application.Commands;
using BetBetBet.Application.Results;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Services;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Presentation.Commands;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class BetCommandExecutorTests
{
    [Fact]
    public void Execute_WinningBet_PrintsWinMessageAndReturnsUpdatedWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var gameEngine = Substitute.For<IGameEngine>();
        var executor = new BetCommandExecutor(console, gameEngine);
        var wallet = new Wallet(Money.Create(10).Value!);
        var command = new BetCommand(Money.Create(5).Value!);

        gameEngine.Play(command.Amount).Returns(new BetOutcome(command.Amount, Money.Create(8).Value!));

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Congrats - you won $8.00! Your current balance is: $13.00");
        result.UpdatedWallet.Should().NotBeNull();
        result.UpdatedWallet!.Balance.Amount.Should().Be(13);
        result.ShouldExit.Should().BeFalse();
    }

    [Fact]
    public void Execute_LosingBet_PrintsLossMessageAndReturnsUpdatedWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var gameEngine = Substitute.For<IGameEngine>();
        var executor = new BetCommandExecutor(console, gameEngine);
        var wallet = new Wallet(Money.Create(10).Value!);
        var command = new BetCommand(Money.Create(5).Value!);

        gameEngine.Play(command.Amount).Returns(new BetOutcome(command.Amount, Money.Create(0).Value!));

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("No luck this time! Your current balance is: $5.00");
        result.UpdatedWallet!.Balance.Amount.Should().Be(5);
    }

    [Fact]
    public void Execute_InsufficientFunds_PrintsErrorAndReturnsNullWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var gameEngine = Substitute.For<IGameEngine>();
        var executor = new BetCommandExecutor(console, gameEngine);
        var wallet = new Wallet(Money.Create(3).Value!);
        var command = new BetCommand(Money.Create(5).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Insufficient funds for bet. Your current balance is: $3.00");
        result.UpdatedWallet.Should().BeNull();
    }

    [Fact]
    public void Execute_OutOfRangeBet_PrintsErrorAndReturnsNullWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var gameEngine = Substitute.For<IGameEngine>();
        var executor = new BetCommandExecutor(console, gameEngine);
        var wallet = new Wallet(Money.Create(100).Value!);
        var command = new BetCommand(Money.Create(15).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Bet must be between $1.00 and $10.00. You tried to bet $15.00.");
        result.UpdatedWallet.Should().BeNull();
    }
}
