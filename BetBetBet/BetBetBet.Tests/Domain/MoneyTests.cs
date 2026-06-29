using BetBetBet.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void Create_ZeroAmount_ReturnsSuccessWithZero()
    {
        // Arrange
        const decimal amount = 0;

        // Act
        var result = Money.Create(amount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(0);
    }

    [Fact]
    public void Create_NegativeAmount_ReturnsFailure()
    {
        // Arrange
        const decimal amount = -1;

        // Act
        var result = Money.Create(amount);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Money.NegativeAmount");
    }

    [Fact]
    public void Equality_SameAmount_AreEqual()
    {
        // Arrange
        var money1 = Money.Create(10).Value!;
        var money2 = Money.Create(10).Value!;

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equality_DifferentAmount_AreNotEqual()
    {
        // Arrange
        var money1 = Money.Create(10).Value!;
        var money2 = Money.Create(20).Value!;

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Addition_TwoAmounts_ReturnsSum()
    {
        // Arrange
        var money1 = Money.Create(10).Value!;
        var money2 = Money.Create(5).Value!;

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(15);
    }

    [Fact]
    public void Subtraction_TwoAmounts_ReturnsDifference()
    {
        // Arrange
        var money1 = Money.Create(10).Value!;
        var money2 = Money.Create(5).Value!;

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(5);
    }

    [Fact]
    public void CompareTo_SameAmount_ReturnsZero()
    {
        // Arrange
        var money1 = Money.Create(10).Value!;
        var money2 = Money.Create(10).Value!;

        // Act
        var result = money1.CompareTo(money2);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CompareTo_LargerAmount_ReturnsPositive()
    {
        // Arrange
        var money1 = Money.Create(20).Value!;
        var money2 = Money.Create(10).Value!;

        // Act
        var result = money1.CompareTo(money2);

        // Assert
        result.Should().BePositive();
    }

    [Fact]
    public void CompareTo_SmallerAmount_ReturnsNegative()
    {
        // Arrange
        var money1 = Money.Create(5).Value!;
        var money2 = Money.Create(10).Value!;

        // Act
        var result = money1.CompareTo(money2);

        // Assert
        result.Should().BeNegative();
    }
}
