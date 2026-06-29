using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Errors;
using BetBetBet.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Application;

public class WithdrawCommandHandlerTests
{
    [Fact]
    public void Handle_ValidWithdraw_ReturnsUpdatedWallet()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(20).Value!);
        var command = new WithdrawCommand(Money.Create(5).Value!);
        var handler = new WithdrawCommandHandler();

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Balance.Amount.Should().Be(15);
    }

    [Fact]
    public void Handle_InsufficientFunds_ReturnsFailure()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(3).Value!);
        var command = new WithdrawCommand(Money.Create(5).Value!);
        var handler = new WithdrawCommandHandler();

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Wallet.InsufficientFunds");
    }
}
