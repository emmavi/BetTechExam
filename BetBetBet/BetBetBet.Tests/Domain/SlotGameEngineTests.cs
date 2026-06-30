using BetBetBet.Domain.Services;
using BetBetBet.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BetBetBet.Tests.Domain;

public class SlotGameEngineTests
{
    [Fact]
    public void Play_LosingTier_ReturnsZeroWin()
    {
        // Arrange
        var random = Substitute.For<IRandomProvider>();
        random.NextDecimal().Returns(0.3m);

        var engine = new SlotGameEngine(random);
        var bet = Money.Create(5).Value!;

        // Act
        var outcome = engine.Play(bet);

        // Assert
        outcome.Win.Amount.Should().Be(0);
        outcome.Bet.Should().Be(bet);
    }

    [Fact]
    public void Play_NormalWinTier_ReturnsWinBetweenZeroAndTwoTimesBet()
    {
        // Arrange
        var random = Substitute.For<IRandomProvider>();
        random.NextDecimal().Returns(0.7m, 0.5m);

        var engine = new SlotGameEngine(random);
        var bet = Money.Create(5).Value!;

        // Act
        var outcome = engine.Play(bet);

        // Assert
        outcome.Win.Amount.Should().BeGreaterThan(0);
        outcome.Win.Amount.Should().BeLessThanOrEqualTo(bet.Amount * 2);
        outcome.Bet.Should().Be(bet);
    }

    [Fact]
    public void Play_BigWinTier_ReturnsWinBetweenTwoAndTenTimesBet()
    {
        // Arrange
        var random = Substitute.For<IRandomProvider>();
        random.NextDecimal().Returns(0.95m, 0.5m);

        var engine = new SlotGameEngine(random);
        var bet = Money.Create(5).Value!;

        // Act
        var outcome = engine.Play(bet);

        // Assert
        outcome.Win.Amount.Should().BeGreaterThan(bet.Amount * 2);
        outcome.Win.Amount.Should().BeLessThanOrEqualTo(bet.Amount * 10);
        outcome.Bet.Should().Be(bet);
    }

    [Fact]
    public void Play_RollExactlyHalf_EntersNormalWinTier()
    {
        // Arrange
        var random = Substitute.For<IRandomProvider>();
        random.NextDecimal().Returns(0.5m, 0.5m);

        var engine = new SlotGameEngine(random);
        var bet = Money.Create(5).Value!;

        // Act
        var outcome = engine.Play(bet);

        // Assert
        outcome.Win.Amount.Should().Be(5m);
        outcome.Bet.Should().Be(bet);
    }

    [Fact]
    public void Play_RollExactlyNinetyPercent_EntersBigWinTier()
    {
        // Arrange
        var random = Substitute.For<IRandomProvider>();
        random.NextDecimal().Returns(0.9m, 0.5m);

        var engine = new SlotGameEngine(random);
        var bet = Money.Create(5).Value!;

        // Act
        var outcome = engine.Play(bet);

        // Assert
        outcome.Win.Amount.Should().Be(30m);
        outcome.Bet.Should().Be(bet);
    }

    [Fact]
    public void Play_RollJustBelowHalf_EntersLosingTier()
    {
        // Arrange
        var random = Substitute.For<IRandomProvider>();
        random.NextDecimal().Returns(0.4999m);

        var engine = new SlotGameEngine(random);
        var bet = Money.Create(5).Value!;

        // Act
        var outcome = engine.Play(bet);

        // Assert
        outcome.Win.Amount.Should().Be(0);
        outcome.Bet.Should().Be(bet);
    }

    [Fact]
    public void Play_RollJustBelowNinetyPercent_EntersNormalWinTier()
    {
        // Arrange
        var random = Substitute.For<IRandomProvider>();
        random.NextDecimal().Returns(0.8999m, 0.5m);

        var engine = new SlotGameEngine(random);
        var bet = Money.Create(5).Value!;

        // Act
        var outcome = engine.Play(bet);

        // Assert
        outcome.Win.Amount.Should().Be(5m);
        outcome.Bet.Should().Be(bet);
    }
}
