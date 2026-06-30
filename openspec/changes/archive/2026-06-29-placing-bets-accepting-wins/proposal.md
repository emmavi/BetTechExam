# Proposal: Placing Bets & Accepting Wins

## Intent

Enable a simple console slot-like round while preserving wallet invariants: bets are $1-$10, cannot exceed current balance, and settle as `new balance = old balance - bet + win`.

## Scope

### In Scope
- Add `bet <amount>` parsing, handling, settlement, and console output.
- Add pure domain betting rules, outcome model, RNG seam, and slot engine.
- Reject invalid or insufficient-funds bets as `Result` business failures.
- Update session behavior so `bet` is supported, not deferred/unknown.
- Cover Domain, Application, Presentation, and Integration paths with deterministic tests.

### Out of Scope
- Persistence, authentication, multi-player state, concurrency, or APIs.
- Deposit/withdraw behavior changes except small shared dispatch/formatting refactors.
- Statistical fairness guarantees beyond deterministic tier coverage.

## Capabilities

### New Capabilities
- `wallet-betting`: Bet validation, game outcomes, wallet settlement, insufficient-funds rejection, and win/loss output.

### Modified Capabilities
- `wallet-session-startup`: Replace deferred/unknown `bet 5` behavior with supported loop handling.

## Approach

Implement the README-aligned vertical slice. Domain owns `BetRules`, `BetOutcome`, `IGameEngine`, `IRandomProvider`, `SlotGameEngine`, and settlement invariants. Application adds `BetCommand`/handler. Infra provides `SystemRandomProvider`. Presentation adds parser registration and exact formatting.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `BetBetBet/BetBetBet.Domain/` | New/Modified | Rules, outcomes, game abstractions, settlement errors. |
| `BetBetBet/BetBetBet.Application/` | New | Bet command, result, handler. |
| `BetBetBet/BetBetBet.Infra/` | New | Concrete random provider. |
| `BetBetBet/BetBetBet.Presentation/` | Modified | `bet` parser, registration, dispatch, formatting. |
| `BetBetBet/BetBetBet.Tests/` | New/Modified | Deterministic layered tests. |
| `openspec/specs/wallet-session-startup/spec.md` | Modified | Remove betting from deferred/unknown behavior. |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Random tier boundaries become ambiguous | Med | Specify threshold scenarios and mock `IRandomProvider`. |
| Console output drifts from README examples | Med | Treat format strings as contract in tests. |
| Wallet balance could go negative | Low | Validate funds before play and enforce Domain settlement invariant. |

## Rollback Plan

Revert betting implementation commit(s), unregister `bet`, and restore `wallet-session-startup` behavior where `bet 5` is unknown/deferred.

## Dependencies

- Existing deposit/withdraw slice and test stack.
- No new external packages.

## Success Criteria

- [ ] `bet` accepts only $1-$10 amounts parsed with invariant culture.
- [ ] Insufficient funds reject without changing balance.
- [ ] Loss, normal win, and big win tiers settle balances correctly.
- [ ] Console loop prints exact win/loss/failure messages and continues after each round.
- [ ] Domain, Application, Presentation, and Integration tests cover the betting flow.
