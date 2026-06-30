# BetTechExam — Player Wallet

Console application in .NET that simulates a player wallet with deposit, withdraw, and a simple slot game.
## Goals

- Demonstrate Clean Architecture with clear layer boundaries.
- Favor explicit, testable design over framework magic.
- Use functional-style error handling (no exceptions for expected business outcomes).
- Keep the domain pure: no I/O, no framework dependencies.

---

## Architecture

Clean Architecture with four production projects plus a test project. Dependencies point inward — outer layers know inner layers, never the reverse.

```
Presentation  ──▶  Application  ──▶  Domain
     │                                  ▲
     ▼                                  │
   Infra ───────────────────────────────┘
```

### Projects

| Project | Responsibility | Knows about |
|--------|----------------|-------------|
| `BetBetBet.Domain` | Entities, value objects, domain services, business rules, error catalog | Nothing — pure C# |
| `BetBetBet.Application` | Use cases, orchestration, command DTOs, result DTOs | Domain |
| `BetBetBet.Infra` | Concrete implementations of infrastructure abstractions (RNG, clock, etc.) | Domain |
| `BetBetBet.Presentation` | Console loop, input/output, formatting | Application, Infra (composition root) |
| `BetBetBet.Tests` | Unit tests, Presentation flow tests, and regression coverage | All of the above |

### Why this split

- **Domain is the heart**: changing input format (console → REST → gRPC) doesn't touch business rules.
- **Application orchestrates use cases**: each business action is one handler — easy to extend (add `history`, `cashout`, etc.).
- **Infra is replaceable**: swap `System.Random` for a cryptographically secure RNG without touching domain.
- **Tests target the right layer**: domain tests are fast and pure; application tests verify orchestration; presentation tests exercise parsing, execution, formatting, and the full console loop.

---

## Error handling

**Rule: do not use exceptions for expected business outcomes.**

Insufficient funds, invalid bet, bad input — these are valid use cases, not exceptional conditions. Exceptions are reserved for programmer errors (null where there should not be one, corrupted state, etc.).

### Result<T> pattern

All operations that can fail in expected ways return `Result<T>`.

```csharp
public sealed class Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }

    [MemberNotNullWhen(true,  nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true,  nameof(Error))]
    public bool IsFailure => !IsSuccess;

    private Result(T value)    { Value = value; }
    private Result(Error error) { Error = error; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T value)    => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
```

**`MemberNotNullWhen` attributes** let the compiler narrow nullability automatically — no `!` null-forgiving operator needed at call sites.

**Implicit conversion operators** let domain methods return raw values or errors directly:

```csharp
public Result<Money> Withdraw(Money amount)
{
    if (amount.Value <= 0)   return WalletErrors.InvalidAmount;
    if (amount > _balance)   return WalletErrors.InsufficientFunds;
    _balance -= amount;
    return _balance;
}
```

### Error type

Errors are records with a stable code plus a human message. Codes are testable; messages are localizable later if needed.

```csharp
public sealed record Error(string Code, string Message);
```

### Error catalog

Errors live as static readonly fields grouped by context.

```csharp
public static class WalletErrors
{
    public static readonly Error InsufficientFunds =
        new("Wallet.InsufficientFunds", "Insufficient funds for withdrawal.");

    public static readonly Error InvalidAmount =
        new("Wallet.InvalidAmount", "Amount must be positive.");
}

public static class BetErrors
{
    public static readonly Error OutOfRange =
        new("Bet.OutOfRange", "Bet must be between $1 and $10.");
}

public static class InputErrors
{
    public static readonly Error UnknownCommand =
        new("Input.UnknownCommand", "Unknown command.");

    public static readonly Error InvalidFormat =
        new("Input.InvalidFormat", "Could not parse amount.");
}
```

---

## Domain

### Value objects

- **`Money`** — wraps `decimal`. Validates non-negative on construction via `Money.Create(decimal) → Result<Money>`. Private constructor. Implements equality, comparison, and arithmetic operators (`+`, `-`, `>`, `<`). Avoids primitive obsession and centralizes money rules.

### Entities

- **`Wallet`** — owns a `Balance` of type `Money`. Public methods:
  - `Deposit(Money) → Result<Money>` — returns new balance
  - `Withdraw(Money) → Result<Money>` — checks sufficient funds
  - `ApplyBetOutcome(BetOutcome) → Result<Money>` — applies `balance - bet + win`
  - Protects invariants: balance is never negative; you cannot apply an outcome larger than balance.

### Domain services

- **`IGameEngine`** — strategy that plays one bet:
  ```csharp
  BetOutcome Play(Money bet);
  ```
- **`SlotGameEngine`** — concrete implementation of the 50/40/10 distribution. Receives `IRandomProvider` via DI to stay testable.
- **`IRandomProvider`** — abstraction over `System.Random` (declared in Domain, implemented in Infra). Enables deterministic tests.

### Domain primitives

- **`BetOutcome`** — record `(Money Bet, Money Win)`. `Win = 0` means loss.
- **`BetRules`** — constants: `MinBet = 1`, `MaxBet = 10`. Static `IsValid(Money)` helper.

---

## Application

### Commands (input DTOs)

Plain records — no behavior. Amount-bearing commands carry validated `Money` values, so handlers do not need to parse console strings or raw decimals.

```csharp
public sealed record DepositCommand(Money Amount) : ICommand;
public sealed record WithdrawCommand(Money Amount) : ICommand;
public sealed record BetCommand(Money Amount) : ICommand;
public sealed record ExitCommand : ICommand;
```

### Handlers (one per use case)

Each handler takes the current `Wallet` plus a command and returns a `Result<T>`. Handlers are the only place that knows the sequence of domain calls for a use case.

- `DepositCommandHandler` — deposits money and returns the updated wallet
- `WithdrawCommandHandler` — withdraws money and returns the updated wallet
- `BetCommandHandler` — validates bet range, calls `IGameEngine.Play`, applies outcome, and returns a `BetResult`

`ExitCommand` has no Application handler today because it is a console-loop concern handled in Presentation.

### Result types

```csharp
public sealed record BetResult(Money Bet, Money Win, Money NewBalance)
{
    public bool IsWin => Win.Value > 0;
}
```

### Dispatching

There is currently no Application-level command dispatcher. Dispatch happens at the Presentation boundary through `CommandExecutorRegistry`, which maps parsed command types to console-specific executors.

An Application-level dispatcher or mediator can be introduced later if another client needs to share the same dispatch policy.

---

## Infra

- **`SystemRandomProvider : IRandomProvider`** — wraps `System.Random`. Singleton in production.
- Composition root wiring (DI registrations) lives here or in Presentation.

---

## Presentation

Owns everything tied to the console medium: reading raw input, parsing it into typed commands, choosing the right console executor, and formatting results for display. If we later swap the console for a REST API or a chat bot, this layer changes — the Application and Domain layers remain focused on use cases and business rules.

### Console loop

Reads input, parses it, executes it through a Presentation executor, applies the returned wallet state, and repeats until an executor signals exit.

### Command parsing

Raw console strings are parsed into typed `ICommand` instances at this boundary. The syntax (`"deposit 10"`) is console-specific — an HTTP layer would deserialize JSON to the same command DTOs instead.

**Design**: each command owns its own parser. The parser registry is keyed by keyword. Adding a new console command means adding a parser class and registering it.

```csharp
public interface ICommandParser
{
    string Keyword { get; }
    Result<ICommand> Parse(string[] args);
}

public sealed class DepositCommandParser : ICommandParser
{
    public string Keyword => "deposit";

    public Result<ICommand> Parse(string[] args)
    {
        if (args.Length != 1) return InputErrors.InvalidFormat;
        if (!decimal.TryParse(args[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            return InputErrors.InvalidFormat;
        if (amount <= 0) return InputErrors.InvalidFormat;
        return new DepositCommand(amount);
    }
}
```

The registry routes by keyword:

```csharp
public sealed class CommandParserRegistry
{
    private readonly Dictionary<string, ICommandParser> _parsers;

    public CommandParserRegistry(IEnumerable<ICommandParser> parsers)
    {
        _parsers = parsers.ToDictionary(p => p.Keyword, StringComparer.OrdinalIgnoreCase);
    }

    public Result<ICommand> Parse(string input)
    {
        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return InputErrors.InvalidFormat;

        if (!_parsers.TryGetValue(parts[0], out var parser))
            return InputErrors.UnknownCommand;

        return parser.Parse(parts[1..]);
    }
}
```

**Rules enforced at this boundary:**
- Case-insensitive keyword matching (`DEPOSIT 10` works).
- Parsed amount strings must be valid invariant-culture decimals.
- Decimal parsing uses `CultureInfo.InvariantCulture` so `10.50` works regardless of locale.
- Unknown keywords return `InputErrors.UnknownCommand`.
- Malformed amount input returns `InputErrors.InvalidFormat`.
- Business invalid amounts, such as zero deposits, are rejected by Domain/Application rules and printed as business error messages.

### Command execution

Parsed commands are executed by console-specific executors in `BetBetBet.Presentation.Commands`. The executor registry is keyed by command type:

```csharp
public interface ICommandExecutor
{
    Type CommandType { get; }
    ExecutionResult Execute(ICommand command, Wallet wallet);
}

public sealed record ExecutionResult(Wallet? UpdatedWallet, bool ShouldExit = false);
```

- `DepositCommandExecutor` calls `DepositCommandHandler` and prints the deposit result.
- `WithdrawCommandExecutor` calls `WithdrawCommandHandler` and prints the withdrawal result.
- `BetCommandExecutor` calls `BetCommandHandler` and delegates win/loss text to `BetResultFormatter`.
- `ExitCommandExecutor` prints the goodbye message and signals loop termination.

This keeps `Program.Run` focused on loop orchestration instead of command-specific branching.

### Output formatting

Format strings match the task PDF exactly:

- `Your deposit of $10 was successful. Your current balance is: $10`
- `Congrats - you won $X.XX! Your current balance is: $Y.YY`
- `No luck this time! Your current balance is: $Y.YY`
- `Your withdrawal of $X.XX was successful. Your current balance is: $Y.YY`
- `Thank you for playing! Hope to see you again soon.`

Failures print the `Error.Message` from the failed `Result`. `BetResultFormatter` keeps bet-specific output formatting separate from the loop and executor wiring.

---

## Testing

### Stack

- **xUnit** — test runner. `[Theory]` + `[InlineData]` for parameterized cases.
- **FluentAssertions** — readable assertions (`balance.Should().Be(10.15m)`).
- **NSubstitute** — mocking, primarily for `IRandomProvider` and other infrastructure abstractions.

### Test layers

#### Domain tests (fast, pure, deterministic)

- `MoneyTests` — construction validation, equality, arithmetic, comparison.
- `WalletTests` — deposit, withdraw with and without sufficient funds, invariants, decimal precision (e.g., the `$0.15` end state from the PDF example).
- `BetRulesTests` — boundary cases at `$1`, `$10`, and outside.
- `SlotGameEngineTests` — with mocked `IRandomProvider`, assert loss and win tiers deterministically.

#### Application tests

- `DepositCommandHandlerTests`, `WithdrawCommandHandlerTests`, `BetCommandHandlerTests` — verify orchestration and `Result` mapping.

#### Presentation tests

- `DepositCommandParserTests`, `WithdrawCommandParserTests`, `BetCommandParserTests`, `ExitCommandParserTests` — one test class per parser, `[Theory]` covering valid input, invalid formats, decimal amounts, negative amounts (rejected).
- `CommandParserRegistryTests` — case-insensitive routing, unknown commands, empty input, whitespace handling.
- `CommandExecutorRegistryTests` — type-based executor lookup and unknown command handling.
- `DepositCommandExecutorTests`, `WithdrawCommandExecutorTests`, `BetCommandExecutorTests`, `ExitCommandExecutorTests` — verify console output, wallet updates, error printing, and exit signaling.
- `SessionRunnerTests` — full console loop flows, including prompt ordering and representative deposit, withdrawal, betting, and exit scenarios.

There is no separate `Integration/` test folder today; full-loop coverage lives under Presentation tests.

### Conventions

- One test class per production class.
- Method naming: `Method_Scenario_ExpectedOutcome` (e.g., `Withdraw_WithInsufficientFunds_ReturnsInsufficientFundsError`).
- `Arrange / Act / Assert` blocks separated by blank lines.
- No `Thread.Sleep`, no real RNG in unit tests — RNG is always mocked.

---

## Folder structure

```
BetBetBet/
├── BetBetBet.slnx
├── BetBetBet.Domain/
│   ├── Common/                 # Result<T>, Error
│   ├── Errors/                 # WalletErrors, BetErrors, InputErrors
│   ├── ValueObjects/           # Money
│   ├── Entities/               # Wallet
│   ├── Services/               # IGameEngine, SlotGameEngine, IRandomProvider, BetRules, BetOutcome
│   └── ...
├── BetBetBet.Application/
│   ├── Commands/               # DepositCommand, WithdrawCommand, BetCommand, ExitCommand
│   ├── Handlers/               # *CommandHandler.cs
│   └── Results/                # BetResult
├── BetBetBet.Infra/
│   └── Random/                 # SystemRandomProvider
├── BetBetBet.Presentation/
│   ├── Program.cs              # composition root + loop
│   ├── Parsing/                # ICommandParser, CommandParserRegistry, *CommandParser
│   ├── Commands/               # ICommandExecutor, CommandExecutorRegistry, *CommandExecutor
│   └── BetResultFormatter.cs   # betting output formatter
└── BetBetBet.Tests/
    ├── Domain/
    ├── Application/
    └── Presentation/
```

---

## Design principles applied

- **Single Responsibility** — one class, one reason to change.
- **Open/Closed** — adding a new console command means adding a command DTO, parser, executor, and handler where needed, instead of growing `Program.Run`.
- **Dependency Inversion** — domain depends on abstractions (`IRandomProvider`, `IGameEngine`); infra provides implementations.
- **Tell, don't ask** — `Wallet` mutates itself; callers don't read balance and recompute outside.
- **Make illegal states unrepresentable** — `Money` cannot be negative; `Wallet` cannot have a negative balance.

## Non-goals

- Persistence — task does not require it.
- Authentication, multi-player, concurrency — single-session console app.
- Full DI container — manual composition is enough for this scope, but the design does not preclude adding `Microsoft.Extensions.DependencyInjection` later.
