# Proposal: Money Withdrawal

## Intent

Enable players to withdraw funds from the session wallet while preserving wallet invariants, README output expectations, and Result-based handling for expected failures.

## Scope

### In Scope
- Add validated `withdraw <amount>` support for positive invariant-culture amounts.
- Decrease wallet balance on success and keep balance non-negative.
- Return insufficient funds as an expected `Result` failure, not an exception, with output that includes current balance.
- Treat malformed withdrawal input consistently with existing parser/input conventions.
- Update startup/command specs that currently classify `withdraw` as unsupported.

### Out of Scope
- Betting, win handling, persistence, authentication, multi-player state, concurrency.
- Larger dispatcher refactor unless strictly required for withdrawal.
- Broad error-catalog cleanup beyond withdrawal needs.

## Capabilities

### New Capabilities
- `wallet-withdrawal`: validated wallet withdrawals, insufficient-funds behavior, success/error display, and withdrawal parser/session behavior.

### Modified Capabilities
- `wallet-session-startup`: `withdraw` becomes a supported wallet command; unsupported-command examples and deferred-operation wording must no longer treat withdrawal as unsupported.

## Approach

Mirror the deposit vertical slice: add `Wallet.Withdraw`, `WithdrawCommand`, `WithdrawCommandHandler`, `WithdrawCommandParser`, registration, session dispatch/output, and focused tests. Keep the current `Program.Run` command branch style for this slice and defer dispatcher extraction.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `BetBetBet/BetBetBet.Domain/Entities/Wallet.cs` | Modified | Add withdrawal business rule and non-negative invariant enforcement. |
| `BetBetBet/BetBetBet.Domain/Errors/` | Modified | Add/use wallet insufficient-funds and withdrawal validation errors as needed. |
| `BetBetBet/BetBetBet.Application/Commands/Handlers` | New | Add withdrawal command and handler. |
| `BetBetBet/BetBetBet.Presentation/Parsing/Program.cs` | Modified | Parse, register, execute, and format withdrawal results. |
| `BetBetBet/BetBetBet.Tests/` | Modified | Add domain, application, parser, registry, and session-flow coverage. |
| `openspec/specs/wallet-session-startup/spec.md` | Modified | Remove withdrawal from unsupported/deferred behavior. |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Output mismatch with README/user decision | Med | Spec exact success format and require failed withdrawal output with current balance. |
| Parser behavior diverges from deposit | Med | Reuse existing input parsing conventions and document withdrawal argument rules. |
| `Program.Run` branching grows | Med | Limit this slice; defer dispatcher refactor to a later change. |

## Rollback Plan

Remove withdrawal command/parser/handler registration and tests, revert `Wallet.Withdraw` and withdrawal specs, and restore startup spec wording that treats `withdraw` as unsupported.

## Dependencies

- Existing deposit/startup slice and README wallet requirements.

## Success Criteria

- [ ] `withdraw <positive amount>` decreases balance and prints `Your withdrawal of $X.XX was successful. Your current balance is: $Y.YY`.
- [ ] Over-withdrawal returns a `Result` failure, preserves balance, and prints current balance.
- [ ] Malformed withdrawal input follows existing input error conventions.
- [ ] Startup specs no longer classify withdrawal as unsupported.
