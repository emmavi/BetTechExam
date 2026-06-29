# Design: Money Deposit

## Technical Approach

Implement a vertical slice across Domain, Application, and Presentation. Add `Wallet.Deposit` as an immutable state transition returning `Result<Wallet>`, introduce `DepositCommand` and `DepositCommandHandler` in Application, add `DepositCommandParser` to the registry, and extend `Program.Run` to dispatch deposits and print the updated balance. Input parsing stays in Presentation; business rules (positive-amount validation and balance mutation) stay in Domain/Application.

## Architecture Decisions

| Decision | Choice | Alternatives | Rationale |
|---|---|---|---|
| Wallet mutability | Immutable (`Deposit` returns `Result<Wallet>`) | Mutable property setter | Matches existing immutable `Money` and value-object style; prevents side effects in tests and enables functional state flow in the session loop. |
| Positive-amount validation | `Wallet.Deposit` rejects `<= 0` | Parser rejects `<= 0` | Keeps the business rule in Domain. Parser only validates format (string → decimal → `Money`). |
| Application handler | Concrete `DepositCommandHandler` class | Direct `Wallet.Deposit` call from `Program` | Establishes the use-case boundary for withdraw/bet later; keeps `Program` as a thin orchestrator. |
| Dispatch style | Type-switch in `Program.Run` | Mediator / dispatcher interface | No DI container or mediator exists yet; type-switch is the minimal extension of the current exit-only dispatch. |

## Data Flow

```
User input ──→ DepositCommandParser ──→ DepositCommand ──→ DepositCommandHandler
     │                                                              │
     └──────────────────────────────────────────────────────────────┘
                              │
                              ↓
                        Wallet.Deposit
                              │
                              ↓
                        Result<Wallet> ──→ Program prints balance or error
```

## File Changes

| File | Action | Description |
|---|---|---|
| `BetBetBet.Domain/Entities/Wallet.cs` | Modify | Add `Deposit(Money amount)` returning `Result<Wallet>`; rejects non-positive amounts. |
| `BetBetBet.Application/Commands/DepositCommand.cs` | Create | Record carrying `Money Amount`; implements `ICommand`. |
| `BetBetBet.Application/Handlers/DepositCommandHandler.cs` | Create | `Handle(Wallet, DepositCommand)` delegates to `Wallet.Deposit`. |
| `BetBetBet.Presentation/Parsing/DepositCommandParser.cs` | Create | Keyword `deposit`; parses single invariant-culture decimal; builds `DepositCommand` via `Money.Create`. |
| `BetBetBet.Presentation/Program.cs` | Modify | Register `DepositCommandParser`; dispatch `DepositCommand` through handler; print balance or error message. |
| `BetBetBet.Tests/Domain/WalletTests.cs` | Modify | Add `Deposit` scenario tests. |
| `BetBetBet.Tests/Presentation/DepositCommandParserTests.cs` | Create | Format, culture, and edge-case parsing tests. |
| `BetBetBet.Tests/Application/DepositCommandHandlerTests.cs` | Create | Success and failure handler tests. |
| `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs` | Modify | Change `deposit 10` unknown-command assertion to a truly unknown command. |
| `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` | Modify | Replace `deposit 10` unknown test with deposit success/error scenarios. |

## Interfaces / Contracts

```csharp
// Domain
public Result<Wallet> Deposit(Money amount);

// Application
public sealed record DepositCommand(Money Amount) : ICommand;
public sealed class DepositCommandHandler
{
    public Result<Wallet> Handle(Wallet wallet, DepositCommand command) { ... }
}

// Presentation
public sealed class DepositCommandParser : ICommandParser
{
    public string Keyword => "deposit";
    public Result<ICommand> Parse(string[] args) { ... }
}
```

## Testing Strategy

| Layer | What to Test | Approach |
|---|---|---|
| Unit (Domain) | `Wallet.Deposit` with positive, zero, and large amounts | Direct invocation; assert new `Balance` or `Error`. |
| Unit (Application) | `DepositCommandHandler` success and failure paths | Direct invocation with a `Wallet` and `DepositCommand`. |
| Unit (Presentation) | `DepositCommandParser` format, culture, missing args | Direct invocation with string arrays; assert command type or error code. |
| Integration (Presentation) | `Program.Run` end-to-end deposit flow | `FakeConsole` sequences (`deposit 10`, `exit`); assert output contains updated balance. |

## Migration / Rollout

No migration required. This is a new behavioral slice on top of the existing session startup.

## Open Questions

None.
