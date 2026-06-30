using BetBetBet.Domain.Services;
using BetBetBet.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Domain;

public class BetRulesTests
{
    [Fact]
    public void IsValid_ZeroAmount_ReturnsFalse()
    {
        // Arrange
        var amount = Money.Create(0).Value!;

        // Act
        var result = BetRules.IsValid(amount);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_MinimumAmount_ReturnsTrue()
    {
        // Arrange
        var amount = Money.Create(1).Value!;

        // Act
        var result = BetRules.IsValid(amount);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_MidRangeAmount_ReturnsTrue()
    {
        // Arrange
        var amount = Money.Create(5).Value!;

        // Act
        var result = BetRules.IsValid(amount);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_MaximumAmount_ReturnsTrue()
    {
        // Arrange
        var amount = Money.Create(10).Value!;

        // Act
        var result = BetRules.IsValid(amount);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_AboveMaximumAmount_ReturnsFalse()
    {
        // Arrange
        var amount = Money.Create(11).Value!;

        // Act
        var result = BetRules.IsValid(amount);

        // Assert
        result.Should().BeFalse();
    }
}
