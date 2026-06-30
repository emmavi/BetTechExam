using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Presentation.Commands;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class WithdrawCommandExecutorTests
{
    [Fact]
    public void Execute_SuccessfulWithdrawal_PrintsSuccessAndReturnsUpdatedWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var executor = new WithdrawCommandExecutor(console);
        var wallet = new Wallet(Money.Create(20).Value!);
        var command = new WithdrawCommand(Money.Create(5).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Your withdrawal of $5.00 was successful. Your current balance is: $15.00");
        result.UpdatedWallet.Should().NotBeNull();
        result.UpdatedWallet!.Balance.Amount.Should().Be(15);
        result.ShouldExit.Should().BeFalse();
    }

    [Fact]
    public void Execute_InsufficientFunds_PrintsErrorAndReturnsNullWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var executor = new WithdrawCommandExecutor(console);
        var wallet = new Wallet(Money.Create(3).Value!);
        var command = new WithdrawCommand(Money.Create(5).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Insufficient funds for withdrawal. Your current balance is: $3.00");
        result.UpdatedWallet.Should().BeNull();
        result.ShouldExit.Should().BeFalse();
    }

    [Fact]
    public void Execute_WithdrawFullBalance_PrintsSuccessAndReturnsZeroBalanceWallet()
    {
        // Arrange
        var console = new FakeConsole();
        var executor = new WithdrawCommandExecutor(console);
        var wallet = new Wallet(Money.Create(20).Value!);
        var command = new WithdrawCommand(Money.Create(20).Value!);

        // Act
        var result = executor.Execute(command, wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Your withdrawal of $20.00 was successful. Your current balance is: $0.00");
        result.UpdatedWallet!.Balance.Amount.Should().Be(0);
    }
}
