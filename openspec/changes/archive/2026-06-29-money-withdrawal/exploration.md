## Exploration: Money withdrawal

### Current State
The current codebase already supports the startup and deposit slices. `Program.Run` creates a wallet at `$0`, registers `exit` and `deposit` parsers, and handles `DepositCommand` inline through `DepositCommandHandler`. `Wallet` is effectively immutable: `Deposit` returns `Result<Wallet>` with a new balance instead of mutating the current instance. There is no withdrawal command, parser, handler, or success/error output yet, and `withdraw 5` is still specified as unsupported in the current startup spec. The README requires positive amounts, exact withdrawal success text, and insufficient-funds handling in the wallet domain, but the current implementation stores deposit-positive validation under `InputErrors` rather than a dedicated wallet error catalog.

### Affected Areas
- `BetBetBet/BetBetBet.Domain/Entities/Wallet.cs` — needs `Withdraw(Money)` while preserving non-negative balance and the current immutable return style.
- `BetBetBet/BetBetBet.Domain/Errors/` — needs a withdrawal error decision; README points to `WalletErrors.InsufficientFunds`, but current deposit code uses `InputErrors` for amount validation.
- `BetBetBet/BetBetBet.Application/Commands/` — needs `WithdrawCommand` following the existing command pattern.
- `BetBetBet/BetBetBet.Application/Handlers/` — needs `WithdrawCommandHandler` mirroring the deposit vertical slice.
- `BetBetBet/BetBetBet.Presentation/Parsing/` — needs `WithdrawCommandParser` with invariant-culture parsing and explicit single-argument validation.
- `BetBetBet/BetBetBet.Presentation/Program.cs` — must register `withdraw`, dispatch it, update wallet state, and print the exact withdrawal success/error output.
- `BetBetBet/BetBetBet.Tests/Domain/WalletTests.cs` — needs withdrawal success, zero, and insufficient-funds scenarios.
- `BetBetBet/BetBetBet.Tests/Application/` — needs `WithdrawCommandHandlerTests`.
- `BetBetBet/BetBetBet.Tests/Presentation/` — needs parser, registry, and session-flow coverage for withdrawal.
- `openspec/specs/wallet-session-startup/spec.md` — must stop treating `withdraw 5` as a generic unsupported example once withdrawal becomes supported.

### Approaches
1. **Mirror the deposit vertical slice** — add `Wallet.Withdraw`, `WithdrawCommand`, `WithdrawCommandHandler`, `WithdrawCommandParser`, and a new `Program.Run` branch.
   - Pros: matches current code shape, keeps the change small, and is the safest continuation from the deposit slice.
   - Cons: repeats the growing `if` chain in `Program.Run` and may preserve the current error-catalog inconsistency.
   - Effort: Medium

2. **Add withdrawal while also introducing a small dispatch abstraction** — implement the withdrawal slice and extract command execution/formatting out of `Program.Run` now.
   - Pros: better aligns with the README's application-dispatch direction before `bet` adds more branching.
   - Cons: broader scope than withdrawal alone and higher risk for this slice.
   - Effort: High

### Recommendation
Use the first approach for this slice: implement withdrawal by mirroring the deposit vertical path, but explicitly decide the error-catalog rule in proposal/design. That keeps the feature reviewable while avoiding an unnecessary dispatcher refactor before the `bet` slice clarifies the broader command model.

### Risks
- The current startup spec still names `withdraw 5` as unsupported, so the next spec phase must modify that source-of-truth behavior.
- `Program.Run` already contains command-specific branching; adding more commands without a later dispatcher cleanup will increase presentation-layer orchestration.
- `DepositCommandParser` currently accepts extra arguments because it only rejects missing args; if withdrawal parser is stricter, command behavior becomes inconsistent unless the proposal addresses parser argument rules.

### Ready for Proposal
Yes — tell the user the codebase is ready for a `money-withdrawal` proposal, with explicit scope for domain insufficient-funds behavior, exact withdrawal output text, and the spec update that removes `withdraw` from unsupported-command examples.
