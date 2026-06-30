using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Presentation.Commands;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class DepositCommandExecutorTests
{
    [Fact]
    public void Execute_SuccessfulDeposit_PrintsSuccessAndReturnsUpdatedWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var executor = new DepositCommandExecutor(console);
        var wallet = new Wallet(Money.Create(0).Value!);
        var command = new DepositCommand(Money.Create(10).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Your deposit of $10 was successful. Your current balance is: $10");
        result.UpdatedWallet.Should().NotBeNull();
        result.UpdatedWallet!.Balance.Amount.Should().Be(10);
        result.ShouldExit.Should().BeFalse();
    }

    [Fact]
    public void Execute_DecimalDeposit_PrintsCorrectBalance()
    {
        // Arrange
        var console = new FakeConsole();
        var executor = new DepositCommandExecutor(console);
        var wallet = new Wallet(Money.Create(10).Value!);
        var command = new DepositCommand(Money.Create(10.50m).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Your deposit of $10.50 was successful. Your current balance is: $20.50");
        result.UpdatedWallet!.Balance.Amount.Should().Be(20.50m);
    }

    [Fact]
    public void Execute_ZeroAmount_PrintsErrorAndReturnsNullWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var executor = new DepositCommandExecutor(console);
        var wallet = new Wallet(Money.Create(0).Value!);
        var command = new DepositCommand(Money.Create(0).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Deposit amount must be positive.");
        result.UpdatedWallet.Should().BeNull();
        result.ShouldExit.Should().BeFalse();
    }

}
