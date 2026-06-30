# Apply Progress: Placing Bets & Accepting Wins

## Workload Decision
- Mode: **size:exception**
- Rationale: Maintainer-approved single PR despite ~650-line forecast.
- All 19 tasks implemented in one batch.

## Completion Status

### Phase 1: Domain Foundation
- [x] 1.1 Create `Domain/Errors/BetErrors.cs` with `OutOfRange(Money,Balance)` and `InsufficientFunds(Money)`
- [x] 1.2 Create `Domain/Services/IRandomProvider.cs` with `decimal NextDecimal()`
- [x] 1.3 Create `Domain/Services/IGameEngine.cs` with `BetOutcome Play(Money bet)`
- [x] 1.4 Create `Domain/Services/BetOutcome.cs` record `(Money Bet, Money Win)`
- [x] 1.5 Create `Domain/Services/BetRules.cs` with `MinBet`/`MaxBet` constants and `IsValid(Money)`
- [x] 1.6 Create `Domain/Services/SlotGameEngine.cs` with 50/40/10 tier logic using `IRandomProvider`
- [x] 1.7 Modify `Domain/Entities/Wallet.cs` — add `ApplyBetOutcome(BetOutcome)` validating `bet <= Balance`

### Phase 2: Application
- [x] 2.1 Create `Application/Commands/BetCommand.cs` record with `Money Amount`
- [x] 2.2 Create `Application/Results/BetResult.cs` record `(Money Bet, Money Win, Money NewBalance, bool IsWin)`
- [x] 2.3 Create `Application/Handlers/BetCommandHandler.cs` — validates bet range, calls engine, settles via `Wallet.ApplyBetOutcome`, returns `Result<BetResult>`

### Phase 3: Infra + Presentation Wiring
- [x] 3.1 Create `Infra/Random/SystemRandomProvider.cs` wrapping `System.Random`
- [x] 3.2 Create `Presentation/Parsing/BetCommandParser.cs` — parses `bet <amount>` with invariant culture
- [x] 3.3 Modify `Presentation/Program.cs` — register parser, dispatch `BetCommand`, format win/loss per spec strings

### Phase 4: Tests
- [x] 4.1 Create `Tests/Domain/SlotGameEngineTests.cs` — tier boundaries with mocked `IRandomProvider`
- [x] 4.2 Create `Tests/Domain/BetRulesTests.cs` — boundary validation for $0, $1, $5, $10, $11
- [x] 4.3 Create `Tests/Application/BetCommandHandlerTests.cs` — validation, play, settlement, error paths
- [x] 4.4 Create `Tests/Presentation/BetCommandParserTests.cs` — valid/invalid `bet` input parsing
- [x] 4.5 Modify `Tests/Presentation/SessionRunnerTests.cs` — add bet success/failure flows, update `bet 5` unknown test
- [x] 4.6 Modify `Tests/Presentation/CommandParserRegistryTests.cs` — update unknown-command assertions for `bet`

## Build & Test Results
- `dotnet build BetBetBet\BetBetBet.slnx`: **PASSED** (0 warnings, 0 errors)
- `dotnet test BetBetBet\BetBetBet.slnx`: **PASSED** (83/83 tests)

## Deviation Log
1. **Infra namespace**: Changed `BetBetBet.Infra.Random` to `BetBetBet.Infra` to avoid namespace/type shadowing conflict with `System.Random`. Used `global::System.Random` in `SystemRandomProvider.cs`.
2. **Infra project reference**: Added missing `BetBetBet.Domain.csproj` reference to `BetBetBet.Infra.csproj`.
3. **BetResult record**: Implemented `IsWin` as a computed property (`public bool IsWin => Win.Amount > 0`) rather than a positional record parameter, per design.md contract.
4. **WalletTests additions**: Added 4 `ApplyBetOutcome` tests to existing `WalletTests.cs` to cover the Domain invariant, even though not explicitly listed in tasks.md — aligned with design.md testing strategy.
5. **SessionRunner bet success test**: Program.cs instantiates real `SystemRandomProvider`, so exact win/loss balance cannot be asserted deterministically. Test verifies command is accepted and produces one of the two valid output messages.

---

## Remediation Pass (Verify Fixes)

**Trigger**: Verify report identified 2 critical untested exact-output scenarios and 1 culture-formatting warning.

### Issues Fixed
1. **Winning bet exact output**: Created `BetResultFormatter.FormatWin` and `BetResultFormatterTests.FormatWin_ExactValues_ReturnsExpectedString` asserting `"Congrats - you won $8.00! Your current balance is: $13.00"`.
2. **Losing bet exact output**: Created `BetResultFormatter.FormatLoss` and `BetResultFormatterTests.FormatLoss_ExactValues_ReturnsExpectedString` asserting `"No luck this time! Your current balance is: $5.00"`.
3. **Culture formatting drift**: Replaced `:F2` in betting output (and withdrawal output for consistency) with explicit `ToString("F2", CultureInfo.InvariantCulture)`.

### Files Changed
| File | Action | What Was Done |
|------|--------|---------------|
| `BetBetBet.Presentation/BetResultFormatter.cs` | Created | Static formatter with deterministic win/loss string builders using InvariantCulture |
| `BetBetBet.Presentation/Program.cs` | Modified | Wired `BetResultFormatter` for win/loss; added `using System.Globalization`; fixed withdrawal line to explicit InvariantCulture formatting |
| `BetBetBet.Tests/Presentation/BetResultFormatterTests.cs` | Created | Two deterministic unit tests asserting exact spec output strings for win and loss scenarios |

### Build & Test Results (Remediation)
- `dotnet build BetBetBet\BetBetBet.slnx`: **PASSED** (0 warnings, 0 errors)
- `dotnet test BetBetBet\BetBetBet.slnx`: **PASSED** (85/85 tests, 2 new)

---

## Boundary Test Pass (Post-Verify Remediation)

**Trigger**: User-requested explicit boundary tests after discussing tier threshold semantics. Production behavior unchanged; tests document current inclusive/exclusive tier boundaries.

### Semantics Documented
- **Losing tier**: `roll < 0.5m`
- **Normal win tier**: `0.5m <= roll < 0.9m`
- **Big win tier**: `roll >= 0.9m`

### Tests Added
| Test | Roll | Expected Tier |
|------|------|---------------|
| `Play_RollExactlyHalf_EntersNormalWinTier` | `0.5m` | Normal win (win = 5 with multiplier 0.5) |
| `Play_RollExactlyNinetyPercent_EntersBigWinTier` | `0.9m` | Big win (win = 30 with multiplier 0.5) |
| `Play_RollJustBelowHalf_EntersLosingTier` | `0.4999m` | Loss (win = 0) |
| `Play_RollJustBelowNinetyPercent_EntersNormalWinTier` | `0.8999m` | Normal win (win = 5 with multiplier 0.5) |

### Files Changed
| File | Action | What Was Done |
|------|--------|---------------|
| `BetBetBet.Tests/Domain/SlotGameEngineTests.cs` | Modified | Added 4 boundary tests with exact-value assertions using mocked `IRandomProvider`; style matches existing xUnit + FluentAssertions + NSubstitute + blank-line-separated Arrange/Act/Assert |

### Build & Test Results (Boundary Pass)
- `dotnet build BetBetBet\BetBetBet.slnx`: **PASSED** (0 warnings, 0 errors)
- `dotnet test BetBetBet\BetBetBet.slnx --filter "FullyQualifiedName~SlotGameEngineTests"`: **PASSED** (7/7 tests)
- `dotnet test BetBetBet\BetBetBet.slnx`: **PASSED** (89/89 tests, 4 new)
