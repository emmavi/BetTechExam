using BetBetBet.Application.Commands;
using BetBetBet.Domain.Errors;
using BetBetBet.Presentation.Parsing;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class CommandParserRegistryTests
{
    [Fact]
    public void Parse_ExitKeyword_ReturnsExitCommand()
    {
        // Arrange
        var registry = new CommandParserRegistry([new ExitCommandParser()]);

        // Act
        var result = registry.Parse("exit");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().BeOfType<ExitCommand>();
    }

    [Fact]
    public void Parse_ExitKeywordCaseInsensitive_ReturnsExitCommand()
    {
        // Arrange
        var registry = new CommandParserRegistry([new ExitCommandParser()]);

        // Act
        var result = registry.Parse("EXIT");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().BeOfType<ExitCommand>();
    }

    [Fact]
    public void Parse_UnknownCommand_ReturnsUnknownCommandError()
    {
        // Arrange
        var registry = new CommandParserRegistry([new ExitCommandParser()]);

        // Act
        var result = registry.Parse("bet 5");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.UnknownCommand.Code);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsUnknownCommandError()
    {
        // Arrange
        var registry = new CommandParserRegistry([new ExitCommandParser()]);

        // Act
        var result = registry.Parse("");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.UnknownCommand.Code);
    }

    [Fact]
    public void Parse_NullInput_ReturnsUnknownCommandError()
    {
        // Arrange
        var registry = new CommandParserRegistry([new ExitCommandParser()]);

        // Act
        var result = registry.Parse(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(InputErrors.UnknownCommand.Code);
    }
}
