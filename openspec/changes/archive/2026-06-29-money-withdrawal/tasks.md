# Tasks: Money Withdrawal

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | 250–350 |
| 400-line budget risk | Low |
| Chained PRs recommended | No |
| Suggested split | Single PR |
| Delivery strategy | ask-on-risk |
| Chain strategy | pending |

Decision needed before apply: No
Chained PRs recommended: No
Chain strategy: pending
400-line budget risk: Low

### Suggested Work Units

| Unit | Goal | Likely PR | Notes |
|------|------|-----------|-------|
| 1 | Domain + Application + Presentation + Tests | PR 1 | Single PR; risk low, under 400 lines |

## Phase 1: Domain Foundation

- [x] 1.1 Create `BetBetBet.Domain/Errors/WalletErrors.cs` with `InsufficientFunds(Money balance)` static factory returning `Error("Wallet.InsufficientFunds", message)`
- [x] 1.2 Add `Wallet.Withdraw(Money amount) → Result<Wallet>` to `BetBetBet.Domain/Entities/Wallet.cs`; reject if `amount > Balance` with `WalletErrors.InsufficientFunds(Balance)`

## Phase 2: Application Layer

- [x] 2.1 Create `BetBetBet.Application/Commands/WithdrawCommand.cs` — `public sealed record WithdrawCommand(Money Amount) : ICommand`
- [x] 2.2 Create `BetBetBet.Application/Handlers/WithdrawCommandHandler.cs` — mirror `DepositCommandHandler`; delegate to `wallet.Withdraw(command.Amount)`

## Phase 3: Presentation Wiring

- [x] 3.1 Create `BetBetBet.Presentation/Parsing/WithdrawCommandParser.cs` — keyword `"withdraw"`; reject missing args, non-numeric, zero, and negative via `InputErrors.InvalidFormat`
- [x] 3.2 Register `WithdrawCommandParser` in `Program.cs`; add `if (parseResult.Value is WithdrawCommand)` branch using `F2` formatting for withdrawal amount and balance

## Phase 4: Testing

- [x] 4.1 Add withdrawal tests to `BetBetBet.Tests/Domain/WalletTests.cs`: `Withdraw_SufficientFunds_ReducesBalance`, `Withdraw_InsufficientFunds_ReturnsError`, `Withdraw_ExactBalance_Succeeds`
- [x] 4.2 Create `BetBetBet.Tests/Application/WithdrawCommandHandlerTests.cs` with success and insufficient-funds paths
- [x] 4.3 Create `BetBetBet.Tests/Presentation/WithdrawCommandParserTests.cs` with valid, missing, invalid, zero, negative, and invariant-decimal cases
- [x] 4.4 Add session tests to `BetBetBet.Tests/Presentation/SessionRunnerTests.cs`: `Run_WithdrawSuccess_UpdatesBalance`, `Run_WithdrawInsufficientFunds_PrintsErrorWithBalance`, `Run_WithdrawInvalidAmount_PrintsError`
