# Tasks: Money Deposit

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~340 |
| 400-line budget risk | Medium |
| Chained PRs recommended | No |
| Suggested split | Single PR |
| Delivery strategy | ask-on-risk |
| Chain strategy | pending |

Decision needed before apply: No
Chained PRs recommended: No
Chain strategy: pending
400-line budget risk: Medium

## Phase 1: Domain Foundation

- [x] 1.1 Add `Wallet.Deposit(Money)` to `BetBetBet.Domain/Entities/Wallet.cs` — returns `Result<Wallet>`, rejects non-positive amounts
- [x] 1.2 Add `DepositAmountMustBePositive` error to `BetBetBet.Domain/Errors/InputErrors.cs`

## Phase 2: Application Layer

- [x] 2.1 Create `DepositCommand` record in `BetBetBet.Application/Commands/DepositCommand.cs` — implements `ICommand`, carries `Money Amount`
- [x] 2.2 Create `DepositCommandHandler` in `BetBetBet.Application/Handlers/DepositCommandHandler.cs` — calls `Wallet.Deposit` and returns the result

## Phase 3: Presentation Layer

- [x] 3.1 Create `DepositCommandParser` in `BetBetBet.Presentation/Parsing/DepositCommandParser.cs` — keyword `deposit`, parses invariant-culture decimal, builds `DepositCommand`
- [x] 3.2 Modify `BetBetBet.Presentation/Program.cs` — register `DepositCommandParser`, dispatch `DepositCommand` via handler, print updated balance or error

## Phase 4: Testing

- [x] 4.1 Add deposit scenarios to `BetBetBet.Tests/Domain/WalletTests.cs` — positive, zero, negative amounts
- [x] 4.2 Create `BetBetBet.Tests/Application/DepositCommandHandlerTests.cs` — success and failure paths
- [x] 4.3 Create `BetBetBet.Tests/Presentation/DepositCommandParserTests.cs` — valid format, missing arg, invalid arg, culture handling
- [x] 4.4 Update `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs` — replace `deposit 10` unknown test with a truly unknown command
- [x] 4.5 Update `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` — replace unknown-deposit test, add deposit success/error end-to-end scenarios
