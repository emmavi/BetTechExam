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
        var console = new FakeConsole("bet 5", "exit");

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

    [Fact]
    public void Run_DepositSuccess_UpdatesBalanceAndContinues()
    {
        // Arrange
        var console = new FakeConsole("deposit 10", "exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Welcome to BetBetBet!",
            "Your current balance is: $0",
            "Please enter a command:",
            "Your deposit of $10 was successful. Your current balance is: $10",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
        console.Outputs.Should().NotContain("Unknown command.");
    }

    [Fact]
    public void Run_AccumulatedDecimalDeposits_UpdatesBalanceToTotalAndContinues()
    {
        // Arrange
        var console = new FakeConsole("deposit 10", "deposit 10.50", "exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Welcome to BetBetBet!",
            "Your current balance is: $0",
            "Please enter a command:",
            "Your deposit of $10 was successful. Your current balance is: $10",
            "Please enter a command:",
            "Your deposit of $10.50 was successful. Your current balance is: $20.50",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
        console.Outputs.Should().NotContain("Unknown command.");
    }

    [Fact]
    public void Run_DepositInvalidAmount_PrintsErrorAndContinues()
    {
        // Arrange
        var console = new FakeConsole("deposit abc", "exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Please enter a command:",
            "Could not parse amount.",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
        console.Outputs.Should().ContainSingle(o => o.StartsWith("Your current balance is:"));
    }

    [Fact]
    public void Run_DepositZeroAmount_PrintsErrorAndContinues()
    {
        // Arrange
        var console = new FakeConsole("deposit 0", "exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Please enter a command:",
            "Deposit amount must be positive.",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
        console.Outputs.Should().ContainSingle(o => o.StartsWith("Your current balance is:"));
    }

    [Fact]
    public void Run_DepositMissingAmount_PrintsErrorAndContinues()
    {
        // Arrange
        var console = new FakeConsole("deposit", "exit");

        // Act
        Program.Run(console);

        // Assert
        console.Outputs.Should().ContainInOrder(
            "Please enter a command:",
            "Could not parse amount.",
            "Please enter a command:",
            "Thank you for playing! Hope to see you again soon."
        );
        console.Outputs.Should().ContainSingle(o => o.StartsWith("Your current balance is:"));
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
