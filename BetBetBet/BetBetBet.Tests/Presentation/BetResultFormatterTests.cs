using BetBetBet.Application.Results;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Presentation;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class BetResultFormatterTests
{
    [Fact]
    public void FormatWin_ExactValues_ReturnsExpectedString()
    {
        // Arrange
        var bet = Money.Create(5).Value!;
        var win = Money.Create(8).Value!;
        var newBalance = Money.Create(13).Value!;
        var result = new BetResult(bet, win, newBalance);

        // Act
        var formatted = BetResultFormatter.FormatWin(result);

        // Assert
        formatted.Should().Be("Congrats - you won $8.00! Your current balance is: $13.00");
    }

    [Fact]
    public void FormatLoss_ExactValues_ReturnsExpectedString()
    {
        // Arrange
        var bet = Money.Create(5).Value!;
        var win = Money.Create(0).Value!;
        var newBalance = Money.Create(5).Value!;
        var result = new BetResult(bet, win, newBalance);

        // Act
        var formatted = BetResultFormatter.FormatLoss(result);

        // Assert
        formatted.Should().Be("No luck this time! Your current balance is: $5.00");
    }
}
