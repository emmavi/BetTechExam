# Verification Report: Money Withdrawal

## Status / Verdict

- **Change**: `money-withdrawal`
- **Mode**: OpenSpec artifact store; Standard verification (`strict_tdd: false`)
- **Verdict**: PASS
- **Archive readiness**: Ready for archive

## Completeness

| Area | Result | Evidence |
|---|---:|---|
| Proposal/spec/design/tasks read | PASS | Read all provided context files before judging implementation. |
| Task completion | PASS | `tasks.md` has 10/10 checked; `apply-progress.md` reports 10/10. |
| Runtime build evidence | PASS | `dotnet build BetBetBet\BetBetBet.slnx` succeeded, 0 warnings, 0 errors. |
| Runtime test evidence | PASS | `dotnet test BetBetBet\BetBetBet.slnx` passed: 53 passed, 0 failed, 0 skipped. |
| Manual runtime output spot-checks | PASS | `dotnet run --project BetBetBet\BetBetBet.Presentation` verified success, insufficient funds, and malformed/non-positive input strings. |

## Build / Test Evidence

| Command | Exit | Evidence |
|---|---:|---|
| `dotnet build BetBetBet\BetBetBet.slnx` | 0 | Build succeeded; 0 warnings; 0 errors. |
| `dotnet test BetBetBet\BetBetBet.slnx` | 0 | Passed: 53; Failed: 0; Skipped: 0; Total: 53. |

Manual runtime checks:

| Input sequence | Observed required output |
|---|---|
| `deposit 20`, `withdraw 5`, `exit` | `Your withdrawal of $5.00 was successful. Your current balance is: $15.00` |
| `deposit 3`, `withdraw 5`, `exit` | `Insufficient funds for withdrawal. Your current balance is: $3.00` |
| `withdraw abc`, `withdraw 0`, `withdraw -1`, `exit` | `Could not parse amount.` for each invalid withdrawal |

## Spec Compliance Matrix

| Spec requirement / scenario | Status | Runtime evidence | Source evidence |
|---|---|---|---|
| Minimal Console Loop — prompts for input | PASS | Existing session tests pass. | `Program.Run` loop writes `Please enter a command:`. |
| Loop continues after unsupported input | PASS | `Run_UnknownCommand_PrintsUnknownAndContinues` passed. | `CommandParserRegistry` returns `InputErrors.UnknownCommand`; loop continues. |
| Loop continues after deposit input | PASS | Deposit session tests pass. | Deposit branch continues after handling. |
| Loop continues after withdrawal input | PASS | `Run_WithdrawSuccess_UpdatesBalance`, `Run_WithdrawInsufficientFunds_PrintsErrorWithBalance`, `Run_WithdrawInvalidAmount_PrintsError` passed. | Withdrawal branch continues after handling. |
| Unknown Command Handling excludes `withdraw` from unsupported commands | PASS | Withdrawal session tests assert output does not contain `Unknown command.` | `Program.cs` registers `new WithdrawCommandParser()`. |
| Deferred Wallet Operations — betting/generic unsupported remain rejected | PASS | Existing unsupported command tests pass. | Unsupported parser fallback still prints `Unknown command.` |
| Successful Withdrawal — balance decreases and never goes negative | PASS | `Run_WithdrawSuccess_UpdatesBalance` and `Withdraw_SufficientFunds_ReducesBalance` passed. | `Wallet.Withdraw` returns `new Wallet(Balance - amount)` only when `amount <= Balance`. |
| Successful withdrawal exact output | PASS | Manual run and `Run_WithdrawSuccess_UpdatesBalance` observed exact required string. | `Program.cs` formats withdrawal amount and balance with `F2`. |
| Malformed amount follows input error convention | PASS | Manual run and `Run_WithdrawInvalidAmount_PrintsError` observed `Could not parse amount.` | `WithdrawCommandParser` returns `InputErrors.InvalidFormat`. |
| Non-positive amount rejected | PASS | Manual run observed `Could not parse amount.` for `withdraw 0` and `withdraw -1`; parser unit tests pass. | `WithdrawCommandParser` rejects `amount <= 0`. |
| Insufficient funds include current balance and preserve balance | PASS | Manual run and `Run_WithdrawInsufficientFunds_PrintsErrorWithBalance` observed exact required string. | `Wallet.Withdraw` returns `WalletErrors.InsufficientFunds(Balance)` before mutation. |

## Correctness Checks

| Check | Status | Evidence |
|---|---|---|
| `withdraw` is no longer unsupported | PASS | Registered parser; runtime/tests show no `Unknown command.` for withdrawal. |
| Invalid/malformed input follows input conventions | PASS | `withdraw abc`, `withdraw 0`, and `withdraw -1` print `Could not parse amount.` |
| Balance never goes negative | PASS | Domain guard rejects `amount > Balance`; insufficient-funds tests preserve original balance. |
| Expected failures use `Result`, not exceptions | PASS | Parser/domain/handler return `Result` failures; tests assert failure codes/messages. |
| Exact success output | PASS | `Your withdrawal of $5.00 was successful. Your current balance is: $15.00` observed. |
| Exact insufficient-funds output | PASS | `Insufficient funds for withdrawal. Your current balance is: $3.00` observed. |

## Design Coherence

| Design decision | Status | Evidence |
|---|---|---|
| Mirror deposit vertical slice | PASS | Added command, handler, parser, registration, domain method, and tests. |
| `Wallet.Withdraw` returns `Result<Wallet>` | PASS | `Wallet.Withdraw(Money amount)` returns `Result<Wallet>`. |
| Domain owns insufficient-funds invariant | PASS | `Wallet.Withdraw` returns `WalletErrors.InsufficientFunds(Balance)`. |
| Withdrawal success uses `F2` formatting in presentation | PASS | `Program.cs` formats success message using `{Amount:F2}`. |
| Parser rejects non-positive values with input error | PASS | `WithdrawCommandParser` returns `InputErrors.InvalidFormat` for `amount <= 0`. |
| Keep current `Program.Run` branching style | PASS | Implementation adds a branch and does not introduce dispatcher refactor. |
| Error balance formatting deviation | PASS WITH NOTE | `WalletErrors.InsufficientFunds` uses invariant `F2` in domain so `Program.Run` can print `result.Error.Message` directly; this is documented in `apply-progress.md` and required for exact spec output. |

## Issues

### CRITICAL

None.

### WARNING

None.

### SUGGESTION

- Consider a future formatting policy so presentation-owned output formatting and domain error messages do not diverge as more wallet operations are added.

## Artifacts

- Written: `openspec/changes/money-withdrawal/verify-report.md`

## Next Recommended

- `sdd-archive`

## Risks

- Low residual risk: insufficient-funds text currently lives in the domain error message to satisfy exact console output. Acceptable for this slice, but worth revisiting if localization or richer presentation formatting appears later.

## Skill Resolution

- `paths-injected` — loaded/read injected SDD shared guidance and used injected `sdd-verify` executor instructions from launch context.
