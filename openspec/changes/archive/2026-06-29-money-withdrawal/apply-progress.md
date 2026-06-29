# Apply Progress: Money Withdrawal

## Status

- **Phase**: apply
- **Mode**: Standard (strict_tdd disabled)
- **Tasks completed**: 10 / 10
- **Build**: Success
- **Tests**: 53 passed, 0 failed

## Completed Tasks

- [x] 1.1 Create `BetBetBet.Domain/Errors/WalletErrors.cs` with `InsufficientFunds(Money balance)` static factory
- [x] 1.2 Add `Wallet.Withdraw(Money amount) → Result<Wallet>` to `BetBetBet.Domain/Entities/Wallet.cs`
- [x] 2.1 Create `BetBetBet.Application/Commands/WithdrawCommand.cs`
- [x] 2.2 Create `BetBetBet.Application/Handlers/WithdrawCommandHandler.cs`
- [x] 3.1 Create `BetBetBet.Presentation/Parsing/WithdrawCommandParser.cs`
- [x] 3.2 Register `WithdrawCommandParser` in `Program.cs`; add withdrawal branch with `F2` formatting
- [x] 4.1 Add withdrawal tests to `WalletTests.cs`
- [x] 4.2 Create `WithdrawCommandHandlerTests.cs`
- [x] 4.3 Create `WithdrawCommandParserTests.cs`
- [x] 4.4 Add session tests to `SessionRunnerTests.cs`

## Files Changed

| File | Action | Description |
|------|--------|-------------|
| `BetBetBet.Domain/Errors/WalletErrors.cs` | Created | `InsufficientFunds(Money balance)` factory with F2-formatted balance in message |
| `BetBetBet.Domain/Entities/Wallet.cs` | Modified | Added `Withdraw(Money) → Result<Wallet>`; rejects if amount > Balance |
| `BetBetBet.Application/Commands/WithdrawCommand.cs` | Created | `public sealed record WithdrawCommand(Money Amount) : ICommand` |
| `BetBetBet.Application/Handlers/WithdrawCommandHandler.cs` | Created | Mirrors `DepositCommandHandler`; delegates to `wallet.Withdraw` |
| `BetBetBet.Presentation/Parsing/WithdrawCommandParser.cs` | Created | Keyword `"withdraw"`; rejects missing/invalid/zero/negative with `InputErrors.InvalidFormat` |
| `BetBetBet.Presentation/Program.cs` | Modified | Registered parser; added `WithdrawCommand` branch with F2 formatting |
| `BetBetBet.Tests/Domain/WalletTests.cs` | Modified | Added `Withdraw_SufficientFunds_ReducesBalance`, `Withdraw_InsufficientFunds_ReturnsError`, `Withdraw_ExactBalance_Succeeds` |
| `BetBetBet.Tests/Application/WithdrawCommandHandlerTests.cs` | Created | Success and insufficient-funds handler tests |
| `BetBetBet.Tests/Presentation/WithdrawCommandParserTests.cs` | Created | Valid, missing, invalid, zero, negative, invariant-decimal cases |
| `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` | Modified | Added `Run_WithdrawSuccess_UpdatesBalance`, `Run_WithdrawInsufficientFunds_PrintsErrorWithBalance`, `Run_WithdrawInvalidAmount_PrintsError` |

## Deviations from Design

**WalletErrors message formatting**: The design states "Choose `F2` in presentation layer" for success messages. For the `InsufficientFunds` error message, the domain error message itself includes F2 formatting (`balance.Amount.ToString("F2", CultureInfo.InvariantCulture)`) because `Program.Run` prints `result.Error.Message` directly for errors, with no presentation-layer interception. This is necessary to produce the exact spec output `Insufficient funds for withdrawal. Your current balance is: $3.00`. The success message still uses F2 exclusively in the presentation layer per the design decision.

## Issues Found

None.

## Test / Build Evidence

- Build command: `dotnet build BetBetBet\BetBetBet.slnx`
- Build result: 0 warnings, 0 errors
- Test command: `dotnet test BetBetBet\BetBetBet.slnx`
- Test result: 53 passed, 0 failed, 0 skipped

## Workload / PR Boundary

- Mode: single PR
- Current work unit: Domain + Application + Presentation + Tests
- Boundary: Complete money-withdrawal vertical slice
- Estimated review budget impact: Under 400 lines (forecast 250–350)

## Next Recommended

sdd-verify
