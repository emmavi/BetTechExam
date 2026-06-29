# Design: Money Withdrawal

## Technical Approach

Mirror the deposit vertical slice: add `Wallet.Withdraw`, a `WithdrawCommand`/`WithdrawCommandHandler`, `WithdrawCommandParser`, register it in `Program.Run`, and add focused tests across Domain, Application, and Presentation. Keep the existing imperative `Program.Run` branching style; defer dispatcher extraction.

## Architecture Decisions

| Decision | Options | Tradeoffs | Rationale |
|---|---|---|---|
| Wallet.Withdraw return type | `Result<Wallet>` (match Deposit) vs `Result<Money>` (README design) | `Result<Wallet>` keeps handler signature identical to deposit; `Result<Money>` would require handler to rebuild wallet | **Choose `Result<Wallet>`** — follows actual codebase pattern, not the README sketch. |
| Insufficient-funds message construction | Wallet constructs it (has balance) vs handler intercepts and re-wraps | Wallet construction is simplest; handler re-wrapping adds indirection for one error | **Choose Wallet constructs Error** — the domain owns the invariant and knows the balance. Add `WalletErrors.InsufficientFunds(Money balance)` factory to keep catalog centralized. |
| Withdrawal amount formatting | Use `Amount.ToString()` (deposit style) vs `Amount.ToString("F2")` (spec example shows `$5.00`) | Raw decimal drops trailing zeros; `F2` always shows two decimals | **Choose `F2` in presentation layer** — the spec scenario explicitly expects `$5.00` for input `withdraw 5`. Deposit remains unchanged. |
| Parser rejects non-positive | Reject at parser (`InputErrors.InvalidFormat`) vs let `Money.Create`/`Wallet.Withdraw` reject | Parser rejection gives uniform "Could not parse amount." for both `abc` and `0`/`-1`; domain rejection would split messages | **Choose parser rejection** — the spec explicitly requires `Could not parse amount.` for zero/negative withdrawal input. |

## Data Flow

```
Console input: "withdraw 5"
    |
    v
CommandParserRegistry ──→ WithdrawCommandParser.Parse(["5"])
    |
    v
Program.Run: if (parseResult.Value is WithdrawCommand cmd)
    |
    v
WithdrawCommandHandler.Handle(wallet, cmd)
    |
    v
wallet.Withdraw(Money amount)
    |
    +-- Success: Result<Wallet> with new balance
    |       → Program prints: "Your withdrawal of $5.00 was successful. Your current balance is: $15.00"
    |
    +-- Failure: WalletErrors.InsufficientFunds(balance)
            → Program prints: "Insufficient funds for withdrawal. Your current balance is: $3.00"
```

## File Changes

| File | Action | Description |
|---|---|---|
| `BetBetBet.Domain/Entities/Wallet.cs` | Modify | Add `Withdraw(Money amount) → Result<Wallet>`; reject if `amount > Balance` with `WalletErrors.InsufficientFunds`. |
| `BetBetBet.Domain/Errors/WalletErrors.cs` | Create | Static factory `InsufficientFunds(Money balance)` producing `Error("Wallet.InsufficientFunds", "Insufficient funds for withdrawal. Your current balance is: ${balance.Amount}")`. |
| `BetBetBet.Application/Commands/WithdrawCommand.cs` | Create | `public sealed record WithdrawCommand(Money Amount) : ICommand;` |
| `BetBetBet.Application/Handlers/WithdrawCommandHandler.cs` | Create | Mirrors `DepositCommandHandler`; delegates to `wallet.Withdraw(command.Amount)`. |
| `BetBetBet.Presentation/Parsing/WithdrawCommandParser.cs` | Create | Keyword `"withdraw"`; `decimal.TryParse` with invariant culture; reject `args.Length == 0`, non-positive amounts, and `Money.Create` failures with `InputErrors.InvalidFormat`. |
| `BetBetBet.Presentation/Program.cs` | Modify | Add `WithdrawCommandParser` to registry; add `if (parseResult.Value is WithdrawCommand)` branch with `F2` formatting. |
| `BetBetBet.Tests/Domain/WalletTests.cs` | Modify | Add `Withdraw_SufficientFunds_ReducesBalance`, `Withdraw_InsufficientFunds_ReturnsError`, `Withdraw_ExactBalance_Succeeds`. |
| `BetBetBet.Tests/Application/WithdrawCommandHandlerTests.cs` | Create | Success and insufficient-funds orchestration tests. |
| `BetBetBet.Tests/Presentation/WithdrawCommandParserTests.cs` | Create | Valid, missing, invalid, negative, zero, and invariant-culture cases. |
| `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` | Modify | Add `Run_WithdrawSuccess_UpdatesBalance`, `Run_WithdrawInsufficientFunds_PrintsErrorWithBalance`, `Run_WithdrawInvalidAmount_PrintsError`. |

## Interfaces / Contracts

```csharp
// Domain
public Result<Wallet> Withdraw(Money amount)
{
    if (amount > Balance)
        return WalletErrors.InsufficientFunds(Balance);
    return new Wallet(Balance - amount);
}

// Error catalog
public static class WalletErrors
{
    public static Error InsufficientFunds(Money balance) =>
        new("Wallet.InsufficientFunds",
            $"Insufficient funds for withdrawal. Your current balance is: ${balance.Amount}");
}

// Presentation formatting (inside Program.Run)
console.WriteLine($"Your withdrawal of {withdrawCommand.Amount.Amount:F2} was successful. Your current balance is: {wallet.Balance.Amount:F2}");
```

## Testing Strategy

| Layer | What to Test | Approach |
|---|---|---|
| Unit — Domain | `Wallet.Withdraw` success, exact-balance, insufficient funds, balance unchanged on failure | xUnit + FluentAssertions; one test class (`WalletTests`), methods per scenario. |
| Unit — Application | `WithdrawCommandHandler` success and failure paths | xUnit; assert `Result.IsSuccess/IsFailure` and error code. |
| Unit — Presentation | `WithdrawCommandParser` valid/invalid/missing/negative/zero/invariant-decimal inputs | xUnit + FluentAssertions; one test class per parser. |
| Integration | Full console loop: valid withdrawal, insufficient funds, invalid input, loop continuation | `FakeConsole` inline stub (existing pattern); assert output sequence and no "Unknown command." |

## Migration / Rollout

No migration required. This is a pure additive slice to an in-memory console session.

## Open Questions

- None. All spec scenarios have a clear implementation path within existing patterns.
