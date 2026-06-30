# Apply Progress: Improve Command Loop with Presentation Pattern

## Workload Mode
- **Mode**: `size:exception`
- **Decision**: User explicitly approved single work unit / single commit (`aprobado para un solo commit`).
- **Changed lines estimate**: ~550–650 (actual within forecast).

## Completion Status

**14/14 tasks complete**

### Phase 1: Foundation Types
- [x] 1.1 Create `Commands/ICommandExecutor.cs`
- [x] 1.2 Create `Commands/ExecutionResult.cs`
- [x] 1.3 Create `Commands/CommandExecutorRegistry.cs`
- [x] 1.4 Create tests/`CommandExecutorRegistryTests.cs`

### Phase 2: Command Executors
- [x] 2.1 Create `Commands/ExitCommandExecutor.cs`
- [x] 2.2 Create `Commands/DepositCommandExecutor.cs`
- [x] 2.3 Create `Commands/WithdrawCommandExecutor.cs`
- [x] 2.4 Create `Commands/BetCommandExecutor.cs`
- [x] 2.5 Create tests/`ExitCommandExecutorTests.cs`
- [x] 2.6 Create tests/`DepositCommandExecutorTests.cs`
- [x] 2.7 Create tests/`WithdrawCommandExecutorTests.cs`
- [x] 2.8 Create tests/`BetCommandExecutorTests.cs`

### Phase 3: Wiring
- [x] 3.1 Modify `Program.cs`
- [x] 3.2 Run all `SessionRunnerTests` — all pass

## Files Changed

| File | Action | Description |
|------|--------|-------------|
| `BetBetBet.Presentation/Commands/ICommandExecutor.cs` | Created | Interface with `CommandType` and `Execute(ICommand, Wallet)` |
| `BetBetBet.Presentation/Commands/ExecutionResult.cs` | Created | Record with `Wallet? UpdatedWallet` and `bool ShouldExit` |
| `BetBetBet.Presentation/Commands/CommandExecutorRegistry.cs` | Created | Dictionary keyed by `Type`; `TryGetExecutor` lookup |
| `BetBetBet.Presentation/Commands/ExitCommandExecutor.cs` | Created | Prints goodbye; returns `ShouldExit = true` |
| `BetBetBet.Presentation/Commands/DepositCommandExecutor.cs` | Created | Delegates to `DepositCommandHandler`; formats deposit success |
| `BetBetBet.Presentation/Commands/WithdrawCommandExecutor.cs` | Created | Delegates to `WithdrawCommandHandler`; formats withdrawal success with F2 |
| `BetBetBet.Presentation/Commands/BetCommandExecutor.cs` | Created | Delegates to `BetCommandHandler`; uses `BetResultFormatter` for win/loss |
| `BetBetBet.Presentation/Program.cs` | Modified | Replaced if-chain with registry composition, dispatch, and result application |
| `BetBetBet.Tests/Presentation/FakeConsole.cs` | Created | Reusable `FakeConsole` for Presentation unit tests |
| `BetBetBet.Tests/Presentation/CommandExecutorRegistryTests.cs` | Created | Registry resolution: known types, unknown types, multiple executors |
| `BetBetBet.Tests/Presentation/ExitCommandExecutorTests.cs` | Created | Goodbye output and exit flag assertions |
| `BetBetBet.Tests/Presentation/DepositCommandExecutorTests.cs` | Created | Success/decimal/error output and wallet update assertions |
| `BetBetBet.Tests/Presentation/WithdrawCommandExecutorTests.cs` | Created | Success/insufficient funds/error output and wallet update assertions |
| `BetBetBet.Tests/Presentation/BetCommandExecutorTests.cs` | Created | Win/loss/insufficient funds/out-of-range output and wallet update assertions |

## Deviations from Design

None — implementation matches design.

## Issues Found

1. **Test construction bug**: Initial `DepositCommandExecutorTests` attempted `Money.Create(-1).Value!` which produces `null` because `Money.Create` returns a failure for negative amounts. This caused a `NullReferenceException` in `Wallet.Deposit`. Removed the unreachable negative-amount executor test since the parser already rejects negative inputs.
2. **Test expectation bug**: Initial `WithdrawCommandExecutorTests` expected `"Could not parse amount."` for a zero-amount withdrawal. However, `Wallet.Withdraw` does not reject zero amounts (the parser does), so the executor would print a success message. Corrected the test to cover a full-balance withdrawal instead.

## Verification

### Build
```
dotnet build BetBetBet\BetBetBet.slnx
```
Result: **0 errors, 0 warnings**

### Tests
```
dotnet test BetBetBet\BetBetBet.slnx
```
Result: **Passed: 103, Failed: 0, Skipped: 0**
- All 14 existing `SessionRunnerTests` pass unchanged.
- All 9 new Presentation executor/registry tests pass.

## TDD Cycle Evidence

N/A — Standard Mode (strict TDD disabled per project config).
