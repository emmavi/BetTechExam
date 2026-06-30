# Tasks: Improve Command Loop with Presentation Pattern

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~550–650 |
| 400-line budget risk | High |
| Chained PRs recommended | Yes |
| Suggested split | PR 1: Foundation → PR 2: Executors → PR 3: Wiring |
| Delivery strategy | ask-on-risk |
| Chain strategy | pending |

Decision needed before apply: Yes
Chained PRs recommended: Yes
Chain strategy: pending
400-line budget risk: High

### Suggested Work Units

| Unit | Goal | Likely PR | Notes |
|------|------|-----------|-------|
| 1 | ICommandExecutor, ExecutionResult, CommandExecutorRegistry + tests | PR 1 | Base types; all executors depend on these |
| 2 | All 4 executors (Exit, Deposit, Withdraw, Bet) + their unit tests | PR 2 | Each executor independent; depends on PR 1 |
| 3 | Wire registry into Program.Run + verify SessionRunnerTests | PR 3 | Replaces if-chain; final behavior lock |

## Phase 1: Foundation Types

- [x] 1.1 Create `Commands/ICommandExecutor.cs` with `Type CommandType` and `ExecutionResult Execute(ICommand, Wallet)`
- [x] 1.2 Create `Commands/ExecutionResult.cs` — record with `Wallet? UpdatedWallet` and `bool ShouldExit`
- [x] 1.3 Create `Commands/CommandExecutorRegistry.cs` — dictionary keyed by `Type`, ctor takes `IEnumerable<ICommandExecutor>`, exposes `TryGetExecutor`
- [x] 1.4 Create tests/`CommandExecutorRegistryTests.cs` — register known executors, resolve by type, reject unknown types

## Phase 2: Command Executors

- [x] 2.1 Create `Commands/ExitCommandExecutor.cs` — prints goodbye, returns `ShouldExit = true`
- [x] 2.2 Create `Commands/DepositCommandExecutor.cs` — calls `DepositCommandHandler`, prints success/error, returns updated Wallet
- [x] 2.3 Create `Commands/WithdrawCommandExecutor.cs` — calls `WithdrawCommandHandler`, prints success/error, returns updated Wallet
- [x] 2.4 Create `Commands/BetCommandExecutor.cs` — calls `BetCommandHandler` + `BetResultFormatter`, prints win/loss/error
- [x] 2.5 Create tests/`ExitCommandExecutorTests.cs` — assert goodbye output and `ShouldExit` flag with `FakeConsole`
- [x] 2.6 Create tests/`DepositCommandExecutorTests.cs` — assert success/error output and wallet update
- [x] 2.7 Create tests/`WithdrawCommandExecutorTests.cs` — assert success/error output and wallet update
- [x] 2.8 Create tests/`BetCommandExecutorTests.cs` — assert win/loss/error output and wallet update

## Phase 3: Wiring

- [x] 3.1 Modify `Program.cs` — compose `CommandExecutorRegistry` with all 4 executors, replace if-chain with registry dispatch, apply `ExecutionResult`
- [x] 3.2 Run all 14 `SessionRunnerTests` — verify unchanged output order, prompt sequence, and wallet behavior
