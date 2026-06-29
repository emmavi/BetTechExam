using BetBetBet.Application.Commands;
using BetBetBet.Presentation.Parsing;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class ExitCommandParserTests
{
    [Fact]
    public void Parse_NoArgs_ReturnsExitCommand()
    {
        // Arrange
        var parser = new ExitCommandParser();

        // Act
        var result = parser.Parse([]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<ExitCommand>();
    }

    [Fact]
    public void Parse_WithExtraArgs_ReturnsFailure()
    {
        // Arrange
        var parser = new ExitCommandParser();

        // Act
        var result = parser.Parse(["now"]);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
