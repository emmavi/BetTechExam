# BetTechExam — Player Wallet

Console application in .NET that simulates a player wallet with deposit, withdraw, and a simple slot game.
## Goals

- Demonstrate Clean Architecture with clear layer boundaries.
- Favor explicit, testable design over framework magic.
- Use functional-style error handling (no exceptions for expected business outcomes).
- Keep the domain pure: no I/O, no framework dependencies.

---

## Architecture

Clean Architecture with four projects. Dependencies point inward — outer layers know inner layers, never the reverse.

```
Presentation  ──▶  Application  ──▶  Domain
     │                                  ▲
     ▼                                  │
   Infra ─────────────────────────────┘
```

### Projects

| Project | Responsibility | Knows about |
|--------|----------------|-------------|
| `BetBetBet.Domain` | Entities, value objects, domain services, business rules, error catalog | Nothing — pure C# |
| `BetBetBet.Application` | Use cases, orchestration, command DTOs, dispatcher | Domain |
| `BetBetBet.Infra` | Concrete implementations of infrastructure abstractions (RNG, clock, etc.) | Domain |
| `BetBetBet.Presentation` | Console loop, input/output, formatting | Application, Infra (composition root) |
| `BetBetBet.Tests` | Unit and integration tests | All of the above |

### Why this split

- **Domain is the heart**: changing input format (console → REST → gRPC) doesn't touch business rules.
- **Application orchestrates**: each use case is one handler — easy to extend (add `history`, `cashout`, etc.).
- **Infra is replaceable**: swap `System.Random` for a cryptographically secure RNG without touching domain.
- **Tests target the right layer**: domain tests are fast and pure; application tests verify orchestration; integration tests exercise the full loop.

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

Plain records — no behavior.

```csharp
public sealed record DepositCommand(decimal Amount);
public sealed record WithdrawCommand(decimal Amount);
public sealed record BetCommand(decimal Amount);
public sealed record ExitCommand;
```

### Handlers (one per use case)

Each handler takes a command and returns a `Result<T>`. Handlers are the only place that knows the sequence of domain calls for a use case.

- `DepositHandler`
- `WithdrawHandler`
- `BetHandler` — validates bet range, calls `IGameEngine.Play`, applies outcome to wallet
- `ExitHandler` — signals loop termination

### Result types

```csharp
public sealed record OperationResult(Money NewBalance);
public sealed record BetResult(Money Bet, Money Win, Money NewBalance)
{
    public bool IsWin => Win.Value > 0;
}
```

### Dispatching

- **`ICommandDispatcher`** — routes a parsed command to the right handler. Simple mediator pattern, no MediatR dependency needed for this scope.

---

## Infra

- **`SystemRandomProvider : IRandomProvider`** — wraps `System.Random`. Singleton in production.
- Composition root wiring (DI registrations) lives here or in Presentation.

---

## Presentation

Owns everything tied to the console medium: reading raw input, parsing it into typed commands, formatting results for display. If we later swap the console for a REST API or a chat bot, only this layer changes — the Application layer remains untouched.

### Console loop

Reads input, parses, dispatches, formats result, prints. Runs until `ExitCommand` is dispatched.

### Command parsing

Raw console strings are parsed into typed `ICommand` instances at this boundary. The syntax (`"deposit 10"`) is console-specific — an HTTP layer would deserialize JSON to the same command DTOs instead.

**Design**: each command owns its own parser. The dispatcher uses a registry keyed by keyword. Adding a new command means adding a parser class and registering it — no changes to existing code (Open/Closed).

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
- Amounts must be a positive decimal (negative values are rejected per the task's "positive number" rule).
- Decimal parsing uses `CultureInfo.InvariantCulture` so `10.50` works regardless of locale.
- Unknown keywords return `InputErrors.UnknownCommand`.
- Malformed input returns `InputErrors.InvalidFormat`.

### Output formatting

Format strings match the task PDF exactly:

- `Your deposit of $10 was successful. Your current balance is: $10`
- `Congrats - you won $X.XX! Your current balance is: $Y.YY`
- `No luck this time! Your current balance is: $Y.YY`
- `Your withdrawal of $X.XX was successful. Your current balance is: $Y.YY`
- `Thank you for playing! Hope to see you again soon.`

Failures print the `Error.Message` from the failed `Result`. An `IResultFormatter` (or pattern matching on the result type) keeps formatting separate from the loop logic.

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
- `SlotGameEngineTests`:
  - With mocked `IRandomProvider`, assert each tier:
    - Random returns `0.3` → loss, `Win = 0`.
    - Random returns `0.7` → win in `[bet, 2 * bet]`.
    - Random returns `0.95` → win in `(2 * bet, 10 * bet]`.
  - Statistical distribution test over N = 10,000 with real RNG and a fixed seed, tolerance ±2%.

#### Application tests

- `DepositHandlerTests`, `WithdrawHandlerTests`, `BetHandlerTests` — verify orchestration: correct domain calls, correct `Result` mapping.

#### Presentation tests

- `DepositCommandParserTests`, `WithdrawCommandParserTests`, `BetCommandParserTests`, `ExitCommandParserTests` — one test class per parser, `[Theory]` covering valid input, invalid formats, decimal amounts, negative amounts (rejected).
- `CommandParserRegistryTests` — case-insensitive routing, unknown commands, empty input, whitespace handling.

#### Integration tests

- Run the full PDF example flow with a stubbed `IRandomProvider` that returns the values needed to reproduce the example sequence. Assert the final balance matches.

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
│   ├── Handlers/               # *Handler.cs
│   ├── Results/                # OperationResult, BetResult
│   └── Dispatching/            # ICommandDispatcher, CommandDispatcher
├── BetBetBet.Infra/
│   └── Random/                 # SystemRandomProvider
├── BetBetBet.Presentation/
│   ├── Program.cs              # composition root + loop
│   ├── Parsing/                # ICommandParser, CommandParserRegistry, *CommandParser
│   └── Formatting/             # output formatters
└── BetBetBet.Tests/
    ├── Domain/
    ├── Application/
    └── Integration/
```

---

## Design principles applied

- **Single Responsibility** — one class, one reason to change.
- **Open/Closed** — adding a new command means adding a handler, not modifying existing ones.
- **Dependency Inversion** — domain depends on abstractions (`IRandomProvider`, `IGameEngine`); infra provides implementations.
- **Tell, don't ask** — `Wallet` mutates itself; callers don't read balance and recompute outside.
- **Make illegal states unrepresentable** — `Money` cannot be negative; `Wallet` cannot have a negative balance.

## Non-goals

- Persistence — task does not require it.
- Authentication, multi-player, concurrency — single-session console app.
- Full DI container — manual composition is enough for this scope, but the design does not preclude adding `Microsoft.Extensions.DependencyInjection` later.
