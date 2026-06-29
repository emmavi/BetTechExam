using BetBetBet.Domain.Entities;
using BetBetBet.Domain.Errors;
using BetBetBet.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Domain;

public class WalletTests
{
    [Fact]
    public void Constructor_ZeroBalance_BalanceIsZero()
    {
        // Arrange
        var money = Money.Create(0).Value!;

        // Act
        var wallet = new Wallet(money);

        // Assert
        wallet.Balance.Amount.Should().Be(0);
    }

    [Fact]
    public void Constructor_NonZeroBalance_BalanceMatchesConstructorValue()
    {
        // Arrange
        var money = Money.Create(100).Value!;

        // Act
        var wallet = new Wallet(money);

        // Assert
        wallet.Balance.Amount.Should().Be(100);
    }

    [Fact]
    public void Deposit_PositiveAmount_IncreasesBalance()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(10).Value!);
        var depositAmount = Money.Create(5).Value!;

        // Act
        var result = wallet.Deposit(depositAmount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Balance.Amount.Should().Be(15);
    }

    [Fact]
    public void Deposit_ZeroAmount_ReturnsFailure()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(10).Value!);
        var depositAmount = Money.Create(0).Value!;

        // Act
        var result = wallet.Deposit(depositAmount);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.DepositAmountMustBePositive.Code);
        wallet.Balance.Amount.Should().Be(10);
    }

    [Fact]
    public void Deposit_NegativeAmount_IsPreventedByMoneyCreation()
    {
        // Arrange
        var moneyResult = Money.Create(-1);

        // Act & Assert
        moneyResult.IsFailure.Should().BeTrue();
        moneyResult.Error!.Code.Should().Be("Money.NegativeAmount");
    }

    [Fact]
    public void Withdraw_SufficientFunds_ReducesBalance()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(20).Value!);
        var withdrawAmount = Money.Create(5).Value!;

        // Act
        var result = wallet.Withdraw(withdrawAmount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Balance.Amount.Should().Be(15);
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ReturnsError()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(3).Value!);
        var withdrawAmount = Money.Create(5).Value!;

        // Act
        var result = wallet.Withdraw(withdrawAmount);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Wallet.InsufficientFunds");
        result.Error!.Message.Should().Be("Insufficient funds for withdrawal. Your current balance is: $3.00");
        wallet.Balance.Amount.Should().Be(3);
    }

    [Fact]
    public void Withdraw_ExactBalance_Succeeds()
    {
        // Arrange
        var wallet = new Wallet(Money.Create(10).Value!);
        var withdrawAmount = Money.Create(10).Value!;

        // Act
        var result = wallet.Withdraw(withdrawAmount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Balance.Amount.Should().Be(0);
    }
}
