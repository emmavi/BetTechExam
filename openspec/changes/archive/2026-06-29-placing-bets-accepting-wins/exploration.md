## Exploration: Placing bets & accepting wins

### Current State
The codebase is not scaffold-only anymore: deposit and withdrawal are already implemented end-to-end across Domain, Application, Presentation, and tests. `Program.Run` owns the console loop, keeps wallet state locally, and dispatches commands via parser/type checks. `Wallet` currently supports immutable `Deposit` and `Withdraw` transitions returning `Result<Wallet>`. There is no betting command, no game engine, no random provider abstraction, no bet outcome model, and no infrastructure code for randomness yet. The source-of-truth OpenSpec still says betting is deferred and `bet 5` is an unknown command, while the README/TASK define the slot-game rules and output expectations.

### Affected Areas
- `BetBetBet/BetBetBet.Domain/Entities/Wallet.cs` — will likely need bet outcome application logic so balance changes remain a domain concern.
- `BetBetBet/BetBetBet.Domain/Errors/` — needs betting-specific business errors such as out-of-range bet and likely insufficient-funds handling for bets.
- `BetBetBet/BetBetBet.Domain/` (new services/primitives) — missing `BetOutcome`, `BetRules`, `IGameEngine`, `SlotGameEngine`, and `IRandomProvider` that the README already anticipates.
- `BetBetBet/BetBetBet.Application/Commands/` — needs a `BetCommand`.
- `BetBetBet/BetBetBet.Application/Handlers/` — needs a `BetCommandHandler` to orchestrate validation, game play, and wallet update.
- `BetBetBet/BetBetBet.Presentation/Parsing/` — needs a `BetCommandParser` with invariant-culture parsing.
- `BetBetBet/BetBetBet.Presentation/Program.cs` — must register `bet`, dispatch it, and print exact win/loss output.
- `BetBetBet/BetBetBet.Infra/` — currently has no C# code; this is the natural place for `SystemRandomProvider`.
- `BetBetBet/BetBetBet.Tests/Domain/` — needs coverage for bet rules, outcome application, and deterministic slot-engine behavior.
- `BetBetBet/BetBetBet.Tests/Application/` — needs `BetCommandHandlerTests`.
- `BetBetBet/BetBetBet.Tests/Presentation/` — needs parser and session-flow tests for `bet`.
- `openspec/specs/wallet-session-startup/spec.md` — must stop treating `bet 5` as unsupported/deferred.
- `openspec/specs/` — likely needs a new betting-focused spec domain for the slot-game behavior.

### Approaches
1. **README-aligned game engine slice** — add betting as a vertical slice with domain game abstractions, deterministic RNG seam, application handler, parser, and presentation formatting.
   - Pros: matches intended architecture, keeps randomness/testability under control, and creates the right foundation for future game behavior.
   - Cons: touches every layer and adds several new types.
   - Effort: High

2. **Minimal inline betting flow** — add `bet` by branching in `Program.Run` and computing outcomes directly there or in a thin handler.
   - Pros: fastest path to make `bet` work.
   - Cons: mixes business rules with presentation, makes probability logic harder to test, and fights the existing README direction.
   - Effort: Medium

### Recommendation
Use the README-aligned game engine slice. Betting introduces randomness, probability tiers, range validation, and wallet settlement, so this is exactly the point where a proper domain service boundary pays off. The existing deposit/withdraw vertical slices already provide the pattern; betting should extend that pattern rather than bypass it.

### Risks
- Current OpenSpec says betting is unsupported, so proposal/spec work must explicitly replace those assumptions.
- `Program.Run` already has growing command-specific branching; adding `bet` without a clearer application dispatch seam will increase presentation orchestration further.
- Current console text differs from `TASK.md` examples (`Please enter a command:` and a welcome banner vs `Please, submit action:`), so output scope must be decided deliberately.
- Existing amount-validation behavior is inconsistent between deposit and withdrawal parsers/domain rules; betting should define clearly whether range/positivity failures belong in parsing or domain validation.
- The SDD init snapshot is stale: test packages are now installed, so future phases should not assume the original “missing test packages” limitation.

### Ready for Proposal
Yes — tell the user the repo is ready for a `placing-bets-accepting-wins` proposal, with explicit scope for bet range validation, deterministic RNG abstractions, exact win/loss output text, wallet settlement rules, and the spec update that removes `bet` from unsupported-command behavior.
