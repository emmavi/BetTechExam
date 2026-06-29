using System.Globalization;
using BetBetBet.Application.Commands;
using BetBetBet.Domain.Errors;
using BetBetBet.Presentation.Parsing;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class DepositCommandParserTests
{
    [Fact]
    public void Parse_ValidWholeAmount_ReturnsDepositCommand()
    {
        // Arrange
        var parser = new DepositCommandParser();

        // Act
        var result = parser.Parse(["10"]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var command = result.Value!.Should().BeOfType<DepositCommand>().Subject;
        command.Amount.Amount.Should().Be(10m);
    }

    [Fact]
    public void Parse_ValidDecimalAmount_ReturnsDepositCommand()
    {
        // Arrange
        var parser = new DepositCommandParser();

        // Act
        var result = parser.Parse(["10.50"]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var command = result.Value!.Should().BeOfType<DepositCommand>().Subject;
        command.Amount.Amount.Should().Be(10.50m);
    }

    [Fact]
    public void Parse_MissingArgs_ReturnsFailure()
    {
        // Arrange
        var parser = new DepositCommandParser();

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
        var parser = new DepositCommandParser();

        // Act
        var result = parser.Parse(["abc"]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.InvalidFormat.Code);
    }

    [Fact]
    public void Parse_NegativeAmount_ReturnsFailure()
    {
        // Arrange
        var parser = new DepositCommandParser();

        // Act
        var result = parser.Parse(["-1"]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Money.NegativeAmount");
    }

    [Fact]
    public void Parse_InvariantCultureDecimal_ReturnsDepositCommand()
    {
        // Arrange
        var parser = new DepositCommandParser();

        // Act
        var result = parser.Parse(["10.50"]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var command = result.Value!.Should().BeOfType<DepositCommand>().Subject;
        command.Amount.Amount.Should().Be(10.50m);
    }
}
