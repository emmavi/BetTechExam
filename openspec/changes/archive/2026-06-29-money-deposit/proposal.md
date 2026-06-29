# Proposal: Money Deposit

## Intent

Enable a player to add funds to the session wallet through a supported `deposit <amount>` command. This replaces the startup-slice assumption that `deposit 10` is unknown, while preserving the local, single-session wallet model.

## Scope

### In Scope
- Add positive decimal deposit behavior to the wallet domain model.
- Add an application deposit command/use case instead of mutating wallet state directly in Presentation.
- Parse `deposit 10` / `deposit 10.50` with invariant-culture decimal handling.
- Reject zero, negative, missing, and invalid deposit amounts with an error message and continue the loop.
- Update session behavior/tests so successful deposit changes the displayed wallet balance.

### Out of Scope
- Withdrawal, betting, win handling, persistence, authentication, concurrency, payment-provider integration, and real-money compliance beyond local command validation.
- Changing unrelated startup, prompt, or exit copy unless required by existing tests/specs.

## Capabilities

### New Capabilities
- `wallet-deposit`: Supports validated local wallet deposits through the console session and application layer.

### Modified Capabilities
- `wallet-session-startup`: `deposit` is no longer an unknown/deferred command; unknown-command and deferred-operation requirements must exclude supported deposit behavior while keeping other unsupported commands rejected.

## Approach

Implement a vertical slice across Domain, Application, and Presentation. Add `Wallet.Deposit` using existing `Money` invariants, introduce a deposit command/handler boundary, register a deposit parser in `CommandParserRegistry`, and route parsed deposit commands through the application use case from the session loop.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `BetBetBet/BetBetBet.Domain/Entities/Wallet.cs` | Modified | Add deposit state transition. |
| `BetBetBet/BetBetBet.Domain/ValueObjects/Money.cs` | Modified | Reuse/adjust validation for positive deposits if needed. |
| `BetBetBet/BetBetBet.Application/Commands/` | New | Add deposit command and handler/use-case pattern. |
| `BetBetBet/BetBetBet.Presentation/Parsing/` | Modified | Add/register deposit parser and validation errors. |
| `BetBetBet/BetBetBet.Presentation/Program.cs` | Modified | Dispatch deposit through Application and print result. |
| `BetBetBet/BetBetBet.Tests/` | Modified | Replace unknown-deposit assertions and add deposit scenarios. |
| `openspec/specs/wallet-session-startup/spec.md` | Modified | Supersede deposit-out-of-scope requirements. |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Contradicting existing startup spec/tests | High | Explicitly modify `wallet-session-startup` in delta specs. |
| Presentation absorbing business rules | Med | Keep parsing in Presentation and wallet mutation in Application/Domain. |
| Output copy mismatch | Med | Preserve existing copy unless specs require exact new messages. |

## Rollback Plan

Revert the deposit parser/registration, deposit application command, wallet deposit method, related tests, and OpenSpec deltas. The session returns to exit-only plus unknown-command behavior.

## Dependencies

- Existing startup slice and `CommandParserRegistry` extension seam.

## Success Criteria

- [ ] `deposit 10` and `deposit 10.50` increase the session wallet balance.
- [ ] Zero, negative, missing, and invalid deposit amounts print an error and keep the loop active.
- [ ] Non-deposit unsupported commands still print `Unknown command.`.
