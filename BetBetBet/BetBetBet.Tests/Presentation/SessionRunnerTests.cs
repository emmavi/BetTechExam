using BetBetBet.Presentation;
using FluentAssertions;
using Xunit;

namespace BetBetBet.Tests.Presentation;

public class SessionRunnerTests
{
    [Fact]
    public void Run_NewSession_DisplaysZeroBalanceBeforeFirstPrompt()
    {
        // Arrange
        var console = new FakeConsole("exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Welcome to BetBetBet!",
            "Your current balance is: $0",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
    }

    [Fact]
    public void Run_ExitCommand_PrintsGoodbyeAndStopsReading()
    {
        // Arrange
        var console = new FakeConsole("exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().Contain("Thank you for playing! Hope to see you again soon.");
        console.ReadCount.Should().Be(1);
    }

    [Fact]
    public void Run_UnknownCommand_PrintsUnknownAndContinues()
    {
        // Arrange
        var console = new FakeConsole("deposit 10", "exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Please enter a command:",
            "Unknown command.",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
    }

    [Fact]
    public void Run_EmptyInput_PrintsUnknownAndContinues()
    {
        // Arrange
        var console = new FakeConsole("", "exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Please enter a command:",
            "Unknown command.",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
    }

    private sealed class FakeConsole : IConsole
    {
        private readonly Queue<string?> _inputs;
        public List<string> Outputs { get; } = new();
        public int ReadCount { get; private set; }

        public FakeConsole(params string?[] inputs)
        {
            _inputs = new Queue<string?>(inputs);
        }

        public void WriteLine(string value) => Outputs.Add(value);

        public string? ReadLine()
        {
            ReadCount++;
            return _inputs.Count > 0 ? _inputs.Dequeue() : null;
        }
    }
}
