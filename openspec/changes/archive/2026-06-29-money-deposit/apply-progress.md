# Apply Progress: Money Deposit

## Mode
Standard (Strict TDD disabled)

## Tasks Completed

### Phase 1: Domain Foundation
- [x] 1.1 Add `Wallet.Deposit(Money)` to `BetBetBet.Domain/Entities/Wallet.cs`
- [x] 1.2 Add `DepositAmountMustBePositive` error to `BetBetBet.Domain/Errors/InputErrors.cs`

### Phase 2: Application Layer
- [x] 2.1 Create `DepositCommand` record in `BetBetBet.Application/Commands/DepositCommand.cs`
- [x] 2.2 Create `DepositCommandHandler` in `BetBetBet.Application/Handlers/DepositCommandHandler.cs`

### Phase 3: Presentation Layer
- [x] 3.1 Create `DepositCommandParser` in `BetBetBet.Presentation/Parsing/DepositCommandParser.cs`
- [x] 3.2 Modify `BetBetBet.Presentation/Program.cs`

### Phase 4: Testing
- [x] 4.1 Add deposit scenarios to `BetBetBet.Tests/Domain/WalletTests.cs`
- [x] 4.2 Create `BetBetBet.Tests/Application/DepositCommandHandlerTests.cs`
- [x] 4.3 Create `BetBetBet.Tests/Presentation/DepositCommandParserTests.cs`
- [x] 4.4 Update `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs`
- [x] 4.5 Update `BetBetBet.Tests/Presentation/SessionRunnerTests.cs`

## Files Changed

| File | Action | What Was Done |
|------|--------|---------------|
| `BetBetBet.Domain/Entities/Wallet.cs` | Modified | Added `Deposit(Money)` returning `Result<Wallet>`; rejects non-positive amounts |
| `BetBetBet.Domain/Errors/InputErrors.cs` | Modified | Added `DepositAmountMustBePositive` error |
| `BetBetBet.Application/Commands/DepositCommand.cs` | Created | Record carrying `Money Amount`; implements `ICommand` |
| `BetBetBet.Application/Handlers/DepositCommandHandler.cs` | Created | Delegates to `Wallet.Deposit` |
| `BetBetBet.Presentation/Parsing/DepositCommandParser.cs` | Created | Parses `deposit <amount>` with invariant-culture decimal |
| `BetBetBet.Presentation/Program.cs` | Modified | Registered parser; dispatches `DepositCommand`; prints balance or error |
| `BetBetBet.Tests/Domain/WalletTests.cs` | Modified | Added `Deposit` positive, zero, and negative-amount-via-Money-creation tests |
| `BetBetBet.Tests/Application/DepositCommandHandlerTests.cs` | Created | Success and failure handler tests |
| `BetBetBet.Tests/Presentation/DepositCommandParserTests.cs` | Created | Valid, missing, invalid, negative, and culture tests |
| `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs` | Modified | Replaced `deposit 10` unknown test with `bet 5` |
| `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` | Modified | Replaced unknown-deposit test; added deposit success/error E2E scenarios |

## Deviations from Design

- **Wallet.Deposit negative-amount test**: Because `Money` has a private constructor and `Money.Create` rejects negative amounts, it is impossible to construct a negative `Money` to pass directly to `Wallet.Deposit`. The `WalletTests` negative-amount scenario verifies that `Money.Create(-1)` fails at the value-object boundary, which prevents negative deposits before they reach the wallet.

## Issues Found

- None.

## Remaining Tasks

- None. All tasks complete.

## Workload / PR Boundary

- Mode: Single PR
- Estimated review budget impact: ~340 changed lines (Medium risk, within 400-line budget)

## Verification

- Build: `dotnet build BetBetBet\BetBetBet.slnx` — succeeded
- Tests: `dotnet test BetBetBet\BetBetBet.slnx` — 37 passed, 0 failed (22 original + 15 new)

## Verification Remediation

### Remediation Batch 1

- Added `Run_AccumulatedDecimalDeposits_UpdatesBalanceToTotalAndContinues` to `SessionRunnerTests.cs` — covers the decimal deposit scenario end-to-end (`$10` + `$10.50` → `$20.50`) and asserts loop continuation.
- Strengthened `Run_DepositSuccess_UpdatesBalanceAndContinues` with `NotContain("Unknown command.")` to prove supported deposit is not treated as unknown.
- Strengthened failure-path session tests (`InvalidAmount`, `ZeroAmount`, `MissingAmount`) with `ContainSingle` on balance lines to prove rejected deposits do not display a new balance.
- Updated `wallet-session-startup/spec.md` to replace the explicit `withdraw 5` scenario with a generic unsupported-command scenario, matching user clarification that withdraw is not a dedicated scope item.
- Corrected task count: 11/11 (was incorrectly reported as 12/12).

## Status

11/11 tasks complete. Ready for verify.
