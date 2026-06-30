using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Presentation.Commands;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class CommandExecutorRegistryTests
{
    [Fact]
    public void TryGetExecutor_RegisteredType_ReturnsExecutor()
    {
        // Arrange
        var executor = new ExitCommandExecutor(new FakeConsole());
        var registry = new CommandExecutorRegistry([executor]);

        // Act
        var found = registry.TryGetExecutor(new ExitCommand(), out var result);

        // Assert
        found.Should().BeTrue();
        result.Should().Be(executor);
    }

    [Fact]
    public void TryGetExecutor_UnknownType_ReturnsFalse()
    {
        // Arrange
        var registry = new CommandExecutorRegistry([]);

        // Act
        var found = registry.TryGetExecutor(new ExitCommand(), out var result);

        // Assert
        found.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetExecutor_MultipleExecutors_ResolvesCorrectType()
    {
        // Arrange
        var exitExecutor = new ExitCommandExecutor(new FakeConsole());
        var depositExecutor = new DepositCommandExecutor(new FakeConsole());
        var registry = new CommandExecutorRegistry([exitExecutor, depositExecutor]);

        // Act
        var foundDeposit = registry.TryGetExecutor(new DepositCommand(Money.Create(1).Value!), out var depositResult);
        var foundExit = registry.TryGetExecutor(new ExitCommand(), out var exitResult);

        // Assert
        foundDeposit.Should().BeTrue();
        depositResult.Should().Be(depositExecutor);
        foundExit.Should().BeTrue();
        exitResult.Should().Be(exitExecutor);
    }
}
