using BetBetBet.Domain.Entities;
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
}
