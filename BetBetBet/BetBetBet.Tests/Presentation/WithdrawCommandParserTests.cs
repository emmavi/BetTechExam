using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Domain.Errors;
using BetBetBet.Presentation.Parsing;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class WithdrawCommandParserTests
{
    [Fact]
    public void Parse_ValidWholeAmount_ReturnsWithdrawCommand()
    {
        // Arrange
        var parser = new WithdrawCommandParser();

        // Act
        var result = parser.Parse(["5"]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var command = result.Value!.Should().BeOfType<WithdrawCommand>().Subject;
        command.Amount.Amount.Should().Be(5m);
    }

    [Fact]
    public void Parse_ValidDecimalAmount_ReturnsWithdrawCommand()
    {
        // Arrange
        var parser = new WithdrawCommandParser();

        // Act
        var result = parser.Parse(["5.50"]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var command = result.Value!.Should().BeOfType<WithdrawCommand>().Subject;
        command.Amount.Amount.Should().Be(5.50m);
    }

    [Fact]
    public void Parse_MissingArgs_ReturnsFailure()
    {
        // Arrange
        var parser = new WithdrawCommandParser();

        // Act
        var result = parser.Parse([]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.InvalidFormat.Code);
    }

    [Fact]
    public void Parse_InvalidAmount_ReturnsFailure()
    {
        // Arrange
        var parser = new WithdrawCommandParser();

        // Act
        var result = parser.Parse(["abc"]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.InvalidFormat.Code);
    }

    [Fact]
    public void Parse_ZeroAmount_ReturnsFailure()
    {
        // Arrange
        var parser = new WithdrawCommandParser();

        // Act
        var result = parser.Parse(["0"]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.InvalidFormat.Code);
    }

    [Fact]
    public void Parse_NegativeAmount_ReturnsFailure()
    {
        // Arrange
        var parser = new WithdrawCommandParser();

        // Act
        var result = parser.Parse(["-1"]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.InvalidFormat.Code);
    }

    [Fact]
    public void Parse_InvariantCultureDecimal_ReturnsWithdrawCommand()
    {
        // Arrange
        var parser = new WithdrawCommandParser();

        // Act
        var result = parser.Parse(["5.50"]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var command = result.Value!.Should().BeOfType<WithdrawCommand>().Subject;
        command.Amount.Amount.Should().Be(5.50m);
    }
}
