using BetBetBet.Application.Commands;
using BetBetBet.Domain.Entities;
using BetBetBet.Domain.ValueObjects;
using BetBetBet.Presentation.Commands;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class ExitCommandExecutorTests
{
    [Fact]
    public void Execute_PrintsGoodbyeAndReturnsShouldExit()
    {
        // Arrange
        var console = new FakeConsole();
        var executor = new ExitCommandExecutor(console);
        var wallet = new Wallet(Money.Create(0).Value!);

        // Act
        var result = executor.Execute(new ExitCommand(), wallet);

        // Assert
        console.Outputs.Should().ContainSingle()
            .Which.Should().Be("Thank you for playing! Hope to see you again soon.");
        result.Should().BeEquivalentTo(new ExecutionResult(null, true));
    }
}
