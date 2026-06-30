using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Errors;
using BetBetBet.Domain.Services;
using BetBetBet.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BetBetBet.Tests.Application;

public class BetCommandHandlerTests
{
    [Fact]
    public void Handle_ValidBetWins_ReturnsBetResultWithUpdatedBalance()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(20).Value!);
        var bet = Money.Create(5).Value!;
        var command = new BetCommand(bet);

        var gameEngine = Substitute.For<IGameEngine>();
        gameEngine.Play(bet).Returns(new BetOutcome(bet, Money.Create(8).Value!));

        var handler = new BetCommandHandler(gameEngine);

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Bet.Amount.Should().Be(5);
        result.Value.Win.Amount.Should().Be(8);
        result.Value.NewBalance.Amount.Should().Be(23);
        result.Value.IsWin.Should().BeTrue();
    }

    [Fact]
    public void Handle_ValidBetLoses_ReturnsBetResultWithReducedBalance()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(20).Value!);
        var bet = Money.Create(5).Value!;
        var command = new BetCommand(bet);

        var gameEngine = Substitute.For<IGameEngine>();
        gameEngine.Play(bet).Returns(new BetOutcome(bet, Money.Create(0).Value!));

        var handler = new BetCommandHandler(gameEngine);

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Bet.Amount.Should().Be(5);
        result.Value.Win.Amount.Should().Be(0);
        result.Value.NewBalance.Amount.Should().Be(15);
        result.Value.IsWin.Should().BeFalse();
    }

    [Fact]
    public void Handle_BetBelowMinimum_ReturnsOutOfRangeError()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(20).Value!);
        var bet = Money.Create(0).Value!;
        var command = new BetCommand(bet);

        var gameEngine = Substitute.For<IGameEngine>();
        var handler = new BetCommandHandler(gameEngine);

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Bet.OutOfRange");
    }

    [Fact]
    public void Handle_BetAboveMaximum_ReturnsOutOfRangeError()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(20).Value!);
        var bet = Money.Create(11).Value!;
        var command = new BetCommand(bet);

        var gameEngine = Substitute.For<IGameEngine>();
        var handler = new BetCommandHandler(gameEngine);

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Bet.OutOfRange");
    }

    [Fact]
    public void Handle_InsufficientFunds_ReturnsInsufficientFundsError()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(3).Value!);
        var bet = Money.Create(5).Value!;
        var command = new BetCommand(bet);

        var gameEngine = Substitute.For<IGameEngine>();
        var handler = new BetCommandHandler(gameEngine);

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Bet.InsufficientFunds");
        result.Error.Message.Should().Be("Insufficient funds for bet. Your current balance is: $3.00");
    }

    [Fact]
    public void Handle_ExactBalanceBet_Succeeds()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(5).Value!);
        var bet = Money.Create(5).Value!;
        var command = new BetCommand(bet);

        var gameEngine = Substitute.For<IGameEngine>();
        gameEngine.Play(bet).Returns(new BetOutcome(bet, Money.Create(0).Value!));

        var handler = new BetCommandHandler(gameEngine);

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.NewBalance.Amount.Should().Be(0);
    }
}
