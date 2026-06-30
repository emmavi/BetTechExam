# Tasks: Placing Bets & Accepting Wins

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~650 |
| 400-line budget risk | High |
| Chained PRs recommended | Yes |
| Suggested split | PR 1 (Domain + Application) → PR 2 (Infra + Presentation + Tests) |
| Delivery strategy | ask-on-risk |
| Chain strategy | pending |

Decision needed before apply: Yes
Chained PRs recommended: Yes
Chain strategy: pending
400-line budget risk: High

### Suggested Work Units

| Unit | Goal | Likely PR | Notes |
|------|------|-----------|-------|
| 1 | Domain betting model + Application layer | PR 1 | Base: feature/tracker branch. Pure types, engine, handler. No infra/presentation. |
| 2 | Infra + Presentation + Integration tests | PR 2 | Base: PR 1 branch. SystemRandomProvider, parser, wiring, remaining tests. |

## Phase 1: Domain Foundation

- [x] 1.1 Create `Domain/Errors/BetErrors.cs` with `OutOfRange(Money,Balance)` and `InsufficientFunds(Money)`
- [x] 1.2 Create `Domain/Services/IRandomProvider.cs` with `decimal NextDecimal()`
- [x] 1.3 Create `Domain/Services/IGameEngine.cs` with `BetOutcome Play(Money bet)`
- [x] 1.4 Create `Domain/Services/BetOutcome.cs` record `(Money Bet, Money Win)`
- [x] 1.5 Create `Domain/Services/BetRules.cs` with `MinBet`/`MaxBet` constants and `IsValid(Money)`
- [x] 1.6 Create `Domain/Services/SlotGameEngine.cs` with 50/40/10 tier logic using `IRandomProvider`
- [x] 1.7 Modify `Domain/Entities/Wallet.cs` — add `ApplyBetOutcome(BetOutcome)` validating `bet <= Balance`

## Phase 2: Application

- [x] 2.1 Create `Application/Commands/BetCommand.cs` record with `Money Amount`
- [x] 2.2 Create `Application/Results/BetResult.cs` record `(Money Bet, Money Win, Money NewBalance, bool IsWin)`
- [x] 2.3 Create `Application/Handlers/BetCommandHandler.cs` — validates bet range, calls engine, settles via `Wallet.ApplyBetOutcome`, returns `Result<BetResult>`

## Phase 3: Infra + Presentation Wiring

- [x] 3.1 Create `Infra/Random/SystemRandomProvider.cs` wrapping `System.Random`
- [x] 3.2 Create `Presentation/Parsing/BetCommandParser.cs` — parses `bet <amount>` with invariant culture
- [x] 3.3 Modify `Presentation/Program.cs` — register parser, dispatch `BetCommand`, format win/loss per spec strings

## Phase 4: Tests

- [x] 4.1 Create `Tests/Domain/SlotGameEngineTests.cs` — tier boundaries with mocked `IRandomProvider`
- [x] 4.2 Create `Tests/Domain/BetRulesTests.cs` — boundary validation for $0, $1, $5, $10, $11
- [x] 4.3 Create `Tests/Application/BetCommandHandlerTests.cs` — validation, play, settlement, error paths
- [x] 4.4 Create `Tests/Presentation/BetCommandParserTests.cs` — valid/invalid `bet` input parsing
- [x] 4.5 Modify `Tests/Presentation/SessionRunnerTests.cs` — add bet success/failure flows, update `bet 5` unknown test
- [x] 4.6 Modify `Tests/Presentation/CommandParserRegistryTests.cs` — update unknown-command assertions for `bet`
