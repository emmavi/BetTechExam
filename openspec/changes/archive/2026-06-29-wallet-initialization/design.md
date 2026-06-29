# Design: Wallet Initialization

## Technical Approach

Implement the minimal domain model (`Money`, `Wallet`, `Result<T>`, `Error`) needed to create a real wallet at `$0`, then wire a tiny presentation seam so the console loop can recognize `exit` and reject everything else. Future commands will plug into the same parser interface without touching the loop.

## Architecture Decisions

| Decision | Options | Tradeoffs | Choice |
|---|---|---|---|
| Domain primitives now? | Introduce `Result<T>` + `Error` now vs defer | Deferring forces a later refactor of `Money.Create` and `Wallet` constructors | Introduce now — they are foundational |
| `Money` value object now? | Use `decimal` directly vs `Money` | `decimal` is less code today, but breaks the "no primitive obsession" rule and forces a refactor when deposit arrives | Introduce `Money` now with `Create(decimal) → Result<Money>` |
| Parser seam vs hardcoded loop | Hardcode `if (input == "exit")` in `Program.cs` vs tiny `ICommandParser` registry | Hardcoding is fewer files; registry adds 3 small classes but keeps Open/Closed for the next slice | Tiny registry with a single `ExitCommandParser` |
| Application layer in this slice? | Add `ExitCommand` DTO vs keep everything in Presentation | A marker DTO costs one file and anchors the command boundary for future handlers | Add `ICommand` + `ExitCommand` in Application |
| Empty input behavior | Return `InvalidFormat` (main spec) vs `UnknownCommand` (delta spec) | Delta spec governs this change; aligning with main spec is a future spec task | Return `UnknownCommand` for empty input in this slice |

## Data Flow

```
Program.cs
  ├── Wallet.Create(Money.Create(0)) ──→ Wallet (Domain)
  ├── Console.WriteLine($"Balance: {wallet.Balance}")
  └── Loop
        ├── Console.ReadLine()
        ├── CommandParserRegistry.Parse(input) ──→ Result<ICommand>
        ├── ExitCommand?  → print goodbye → break
        └── Error / other? → print "Unknown command." → continue
```

## File Changes

| File | Action | Description |
|------|--------|-------------|
| `BetBetBet.Domain/Common/Result.cs` | Create | Generic `Result<T>` with `MemberNotNullWhen` and implicit conversions per README. |
| `BetBetBet.Domain/Common/Error.cs` | Create | `record Error(string Code, string Message)`. |
| `BetBetBet.Domain/ValueObjects/Money.cs` | Create | `Money.Create(decimal) → Result<Money>`, private ctor, equality, comparison, `+`/`-` operators. |
| `BetBetBet.Domain/Entities/Wallet.cs` | Create | Constructor takes `Money balance`. Exposes `Balance`. `Deposit`/`Withdraw`/`ApplyBetOutcome` deferred. |
| `BetBetBet.Application/Commands/ICommand.cs` | Create | Marker interface for parsed commands. |
| `BetBetBet.Application/Commands/ExitCommand.cs` | Create | `sealed record ExitCommand : ICommand;` |
| `BetBetBet.Presentation/Parsing/ICommandParser.cs` | Create | `string Keyword { get; }`, `Result<ICommand> Parse(string[] args);` |
| `BetBetBet.Presentation/Parsing/ExitCommandParser.cs` | Create | Keyword `exit`, returns `ExitCommand`. |
| `BetBetBet.Presentation/Parsing/CommandParserRegistry.cs` | Create | Routes by keyword (case-insensitive). Empty/unknown input returns `InputErrors.UnknownCommand`. |
| `BetBetBet.Presentation/Program.cs` | Create | Composition root: create wallet, print balance, run read-dispatch loop. |
| `BetBetBet.Tests/BetBetBet.Tests.csproj` | Modify | Add `xUnit`, `FluentAssertions`, `NSubstitute` packages. |
| `BetBetBet.Tests/Domain/MoneyTests.cs` | Create | Construction, equality, arithmetic, `Create(negative)` returns error. |
| `BetBetBet.Tests/Domain/WalletTests.cs` | Create | `Wallet` initialized with `$0`; `Balance` matches constructor value. |
| `BetBetBet.Tests/Presentation/ExitCommandParserTests.cs` | Create | Parses `exit`, rejects args. |
| `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs` | Create | Routes `exit` (case-insensitive), unknown/empty input returns `UnknownCommand`. |

## Interfaces / Contracts

```csharp
// Domain
public sealed class Result<T> { /* MemberNotNullWhen, implicit ops */ }
public sealed record Error(string Code, string Message);
public sealed record Money : IComparable<Money> { /* private ctor, operators */ }

public sealed class Wallet
{
    public Money Balance { get; }
    public Wallet(Money balance) { Balance = balance; }
}

// Application
public interface ICommand { }
public sealed record ExitCommand : ICommand;

// Presentation
public interface ICommandParser
{
    string Keyword { get; }
    Result<ICommand> Parse(string[] args);
}

public sealed class CommandParserRegistry
{
    public Result<ICommand> Parse(string input) { /* case-insensitive routing */ }
}
```

## Testing Strategy

| Layer | What to Test | Approach |
|-------|-------------|----------|
| Unit — Domain | `Money.Create`, equality, arithmetic; `Wallet` construction | Pure, deterministic. No mocks needed. |
| Unit — Presentation | `ExitCommandParser`, `CommandParserRegistry` routing and error cases | Direct instantiation; no I/O. |
| Integration | Full console loop | Deferred to later slice — not enough application behavior to justify an integration test yet. |

**Note**: `BetBetBet.Tests.csproj` currently has **zero NuGet packages**. The first task in the apply phase must add `xUnit`, `FluentAssertions`, and `NSubstitute` before any test code compiles.

## Migration / Rollout

No migration required. This is the first runnable slice.

## Open Questions

- Should `Program.cs` extract an `IConsole` abstraction now for testability, or defer until integration tests are written? Deferring keeps the slice smaller; the loop is simple enough to test via parser/registry unit tests.
- The delta spec returns `UnknownCommand` for empty input, while the README main spec returns `InvalidFormat`. Which spec wins in the final merge? This slice follows the delta; archive phase should reconcile.
