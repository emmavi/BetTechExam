using BetBetBet.Application.Commands;
using BetBetBet.Application.Handlers;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Errors;
using BetBetBet.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Application;

public class DepositCommandHandlerTests
{
    [Fact]
    public void Handle_ValidDeposit_ReturnsUpdatedWallet()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(0).Value!);
        var command = new DepositCommand(Money.Create(10).Value!);
        var handler = new DepositCommandHandler();

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Balance.Amount.Should().Be(10);
    }

    [Fact]
    public void Handle_ZeroDeposit_ReturnsFailure()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(5).Value!);
        var command = new DepositCommand(Money.Create(0).Value!);
        var handler = new DepositCommandHandler();

        // Act
        var result = handler.Handle(wallet, command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.DepositAmountMustBePositive.Code);
    }
}
