# Design: Improve Command Loop with Presentation Pattern

## Technical Approach

Replace the command-specific `if` chain in `Program.Run` with a Presentation-level executor registry keyed by parsed command type. Each executor adapts the existing Application handler, formats console output, and returns a normalized `ExecutionResult` (updated wallet + exit flag). `Program.Run` keeps composition-root wiring and the read loop; executors own per-command behavior.

## Architecture Decisions

| Decision | Options | Tradeoffs | Choice |
|----------|---------|-----------|--------|
| Executor abstraction | Non-generic `ICommandExecutor` keyed by `Type` vs generic `ICommandExecutor<T>` | Generic avoids casts but needs more wiring and a non-generic wrapper; non-generic mirrors the existing parser registry exactly. | Non-generic keyed by `Type` |
| Result shape | `ExecutionResult` record vs mutable `SessionContext` | `SessionContext` is extensible but encourages side effects; `ExecutionResult` is explicit and matches current wallet reassignment in `Program.Run`. | `ExecutionResult` record with `Wallet? UpdatedWallet` and `bool ShouldExit` |
| Console output location | Inside executor vs returned in result | Returning strings adds allocation for zero behavior gain; current code already writes directly in branches. | Keep console writes as executor side effects |

## Data Flow

```
Input → CommandParserRegistry.Parse → ICommand
                                      ↓
                   CommandExecutorRegistry.TryGetExecutor(Type)
                                      ↓
                          ICommandExecutor.Execute(wallet)
                                      ↓
                     ExecutionResult(Wallet?, ShouldExit)
                                      ↓
              Program.Run updates wallet or breaks loop
```

## File Changes

| File | Action | Description |
|------|--------|-------------|
| `BetBetBet.Presentation/Commands/ICommandExecutor.cs` | Create | `Type CommandType { get; }` and `ExecutionResult Execute(ICommand, Wallet)`. |
| `BetBetBet.Presentation/Commands/ExecutionResult.cs` | Create | `record ExecutionResult(Wallet? UpdatedWallet, bool ShouldExit)`. |
| `BetBetBet.Presentation/Commands/CommandExecutorRegistry.cs` | Create | Registry dictionary keyed by `Type`; constructor takes `IEnumerable<ICommandExecutor>`. |
| `BetBetBet.Presentation/Commands/ExitCommandExecutor.cs` | Create | Prints goodbye; returns `ShouldExit = true`. |
| `BetBetBet.Presentation/Commands/DepositCommandExecutor.cs` | Create | Uses `DepositCommandHandler`, prints deposit success/error. |
| `BetBetBet.Presentation/Commands/WithdrawCommandExecutor.cs` | Create | Uses `WithdrawCommandHandler`, prints withdrawal success/error. |
| `BetBetBet.Presentation/Commands/BetCommandExecutor.cs` | Create | Uses `BetCommandHandler` + `BetResultFormatter`, prints win/loss/error. |
| `BetBetBet.Presentation/Program.cs` | Modify | Replace `if` chain with registry composition, resolve, execute, apply result. |
| `BetBetBet.Tests/Presentation/ExitCommandExecutorTests.cs` | Create | Assert goodbye output and exit flag. |
| `BetBetBet.Tests/Presentation/DepositCommandExecutorTests.cs` | Create | Assert success/error output and wallet update. |
| `BetBetBet.Tests/Presentation/WithdrawCommandExecutorTests.cs` | Create | Assert success/error output and wallet update. |
| `BetBetBet.Tests/Presentation/BetCommandExecutorTests.cs` | Create | Assert win/loss/error output and wallet update. |
| `BetBetBet.Tests/Presentation/CommandExecutorRegistryTests.cs` | Create | Resolve by type, reject unknown command types. |
| `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` | Modify | Keep existing tests as integration lock; no behavior change expected. |

## Interfaces / Contracts

```csharp
public interface ICommandExecutor
{
    Type CommandType { get; }
    ExecutionResult Execute(ICommand command, Wallet currentWallet);
}

public sealed record ExecutionResult(Wallet? UpdatedWallet, bool ShouldExit);
```

`CommandExecutorRegistry` exposes:

```csharp
public bool TryGetExecutor(ICommand command, out ICommandExecutor? executor);
```

Executors receive dependencies via constructor (`IConsole`, `IGameEngine` for `BetCommandExecutor`) to keep `Execute` stateless.

## Testing Strategy

| Layer | What to Test | Approach |
|-------|-------------|----------|
| Unit | Each executor | Inject `FakeConsole`; assert exact output strings and `ExecutionResult` shape. |
| Unit | `CommandExecutorRegistry` | Register executors, resolve known types, reject unknown types. |
| Integration | `SessionRunnerTests` | Existing tests remain authoritative; verify prompt order and wallet updates unchanged. |

## Migration / Rollout

No migration required. Behavior-preserving refactor; no data or spec changes.

## Open Questions

None.
