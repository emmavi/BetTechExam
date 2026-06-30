# Design: Placing Bets & Accepting Wins

## Technical Approach

Extend the existing deposit/withdraw vertical slice with a `bet` command. Domain adds game abstractions (`IRandomProvider`, `IGameEngine`, `SlotGameEngine`, `BetOutcome`, `BetRules`) and a `Wallet.ApplyBetOutcome` settlement method. Application adds `BetCommand`, `BetCommandHandler` returning `Result<BetResult>`, and `BetResult`. Infra adds `SystemRandomProvider`. Presentation adds `BetCommandParser`, registers it, and formats win/loss output per the wallet-betting spec. Tests use mocked `IRandomProvider` for determinism.

## Architecture Decisions

| Decision | Options | Tradeoffs | Choice |
|---|---|---|---|
| Where to place RNG abstraction | Domain vs Application | Domain keeps game logic pure; Application would couple engine to outer layer | **Domain** (`IRandomProvider`) |
| Handler return type for betting | `Result<Wallet>` vs `Result<BetResult>` | `Wallet` lacks win metadata; `BetResult` adds a type but is needed for output | **`Result<BetResult>`** |
| Bet range validation | Parser vs Handler | Parser keeps syntax simple; Handler treats it as business rule | **Handler** (business failure) |
| Wallet settlement API | `PlaceBet(bet,win)` vs `ApplyBetOutcome(outcome)` | `ApplyBetOutcome` aligns with README and keeps intent clear | **`ApplyBetOutcome`** |
| Insufficient-funds guard | Handler only vs Handler + Domain | Handler gives early user-facing failure; Domain invariant protects against every other caller | **Both**: Handler pre-checks with `BetErrors.InsufficientFunds`, `Wallet.ApplyBetOutcome` enforces `bet <= Balance` and returns the same betting-specific error |

## Data Flow

```
Console input ──→ BetCommandParser ──→ BetCommand
                                            │
                                            ▼
Wallet ──→ BetCommandHandler ──→ IGameEngine.Play(bet)
   ▲              │                       │
   │              ▼                       ▼
   └── Result<BetResult> ←── Wallet.ApplyBetOutcome ←── BetOutcome
                            │
                            ▼
                    Presentation formats win/loss
```

## File Changes

| File | Action | Description |
|---|---|---|
| `Domain/Services/IRandomProvider.cs` | Create | Abstraction for deterministic randomness |
| `Domain/Services/IGameEngine.cs` | Create | `BetOutcome Play(Money bet)` |
| `Domain/Services/SlotGameEngine.cs` | Create | 50/40/10 tier logic |
| `Domain/Services/BetRules.cs` | Create | Min/Max constants and `IsValid` |
| `Domain/Services/BetOutcome.cs` | Create | Record `(Money Bet, Money Win)` |
| `Domain/Errors/BetErrors.cs` | Create | `OutOfRange`, `InsufficientFunds` |
| `Domain/Entities/Wallet.cs` | Modify | Add `ApplyBetOutcome`; validates `bet <= Balance` as Domain invariant |
| `Application/Commands/BetCommand.cs` | Create | Record with `Money Amount` |
| `Application/Results/BetResult.cs` | Create | Record `(Money Bet, Money Win, Money NewBalance)` |
| `Application/Handlers/BetCommandHandler.cs` | Create | Validates, plays, settles |
| `Infra/Random/SystemRandomProvider.cs` | Create | Wraps `System.Random` |
| `Presentation/Parsing/BetCommandParser.cs` | Create | Parses `bet <amount>` |
| `Presentation/Program.cs` | Modify | Register parser, dispatch `BetCommand`, format output |
| `Tests/Domain/SlotGameEngineTests.cs` | Create | Tier behavior with mocked RNG |
| `Tests/Domain/BetRulesTests.cs` | Create | Boundary validation |
| `Tests/Application/BetCommandHandlerTests.cs` | Create | Orchestration and error paths |
| `Tests/Presentation/BetCommandParserTests.cs` | Create | Parsing valid/invalid input |
| `Tests/Presentation/SessionRunnerTests.cs` | Modify | Add bet success/failure flows; update `bet 5` unknown tests |
| `Tests/Presentation/CommandParserRegistryTests.cs` | Modify | Update unknown-command test |

## Interfaces / Contracts

```csharp
public interface IRandomProvider { decimal NextDecimal(); }

public interface IGameEngine { BetOutcome Play(Money bet); }

public sealed record BetOutcome(Money Bet, Money Win);

public static class BetRules
{
    public static readonly Money MinBet = Money.Create(1).Value!;
    public static readonly Money MaxBet = Money.Create(10).Value!;
    public static bool IsValid(Money amount) => amount >= MinBet && amount <= MaxBet;
}

public sealed record BetResult(Money Bet, Money Win, Money NewBalance)
{
    public bool IsWin => Win.Amount > 0;
}

// Wallet settlement invariant: enforces bet <= Balance independently of handler pre-check
public Result<Wallet> ApplyBetOutcome(BetOutcome outcome)
{
    if (outcome.Bet > Balance) return BetErrors.InsufficientFunds(Balance);
    return new Wallet(Balance - outcome.Bet + outcome.Win);
}
```

## Testing Strategy

| Layer | What to Test | Approach |
|---|---|---|
| Unit (Domain) | `SlotGameEngine` tier mapping, `BetRules` boundaries, `Wallet.ApplyBetOutcome` (including insufficient-funds invariant) | Mock `IRandomProvider` with NSubstitute; assert exact `BetOutcome` |
| Unit (Application) | `BetCommandHandler` validation, orchestration, error paths | Direct invocation with stubbed `IGameEngine` |
| Unit (Presentation) | `BetCommandParser` parsing, `Program.Run` output formatting | FakeConsole, assert exact strings |
| Integration | Full loop: deposit → bet → withdraw → exit | FakeConsole with seeded `SystemRandomProvider` |

## Migration / Rollout

No migration required. This is a net-new feature. Existing `CommandParserRegistryTests` and `SessionRunnerTests` that treat `bet 5` as unknown must be updated to reflect the new supported command.

## Open Questions

None.
