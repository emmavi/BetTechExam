## Exploration: Money deposit

### Current State
The codebase already implements a small startup slice, not empty scaffolding. `Program.Run` creates a zero-balance wallet, prints the balance, parses commands through `CommandParserRegistry`, and only supports `exit`; every other input is treated as `Unknown command.`. The domain currently has `Money` plus a minimal `Wallet` that only stores `Balance` and has no deposit behavior. There are no deposit command DTOs, handlers, dispatcher, or formatters yet, and the existing OpenSpec main spec explicitly marks deposit as out of scope for the startup slice.

### Affected Areas
- `BetBetBet/BetBetBet.Domain/Entities/Wallet.cs` — needs deposit behavior and balance state changes.
- `BetBetBet/BetBetBet.Domain/ValueObjects/Money.cs` — already enforces non-negative money and will be reused by deposit validation.
- `BetBetBet/BetBetBet.Application/Commands/` — needs a `DepositCommand` and likely supporting result type/handler abstractions.
- `BetBetBet/BetBetBet.Presentation/Program.cs` — currently owns the loop and wallet directly; deposit handling must be wired here or behind an application boundary.
- `BetBetBet/BetBetBet.Presentation/Parsing/CommandParserRegistry.cs` — registry already supports extension by keyword and can register a deposit parser.
- `BetBetBet/BetBetBet.Presentation/Parsing/` — needs a `DepositCommandParser` using invariant-culture decimal parsing and positive-amount validation.
- `BetBetBet/BetBetBet.Tests/Presentation/SessionRunnerTests.cs` — current tests assert `deposit 10` is unknown; these will need to change for the new slice.
- `openspec/specs/wallet-session-startup/spec.md` — current source-of-truth spec explicitly says deposit is deferred/out of scope.

### Approaches
1. **Vertical deposit slice across Domain/Application/Presentation** — add `Wallet.Deposit`, `DepositCommand`, a deposit handler/use-case, a deposit parser, and success/error output in the loop.
   - Pros: matches the README architecture, keeps business rules out of Presentation, and creates the pattern needed for withdraw/bet later.
   - Cons: slightly more code now because Application is still mostly absent.
   - Effort: Medium

2. **Presentation-first shortcut** — parse `deposit` in Presentation and update the wallet directly from `Program.Run` without introducing handlers yet.
   - Pros: fastest path for a single feature.
   - Cons: breaks the intended Clean Architecture direction, increases future refactor cost, and duplicates orchestration logic that withdraw/bet will also need.
   - Effort: Low

### Recommendation
Use the vertical deposit slice. The repository already has the extension seam at the parser registry, but the missing application layer should be introduced now so deposit becomes the first real use case and establishes the pattern for withdraw and bet instead of forcing a later rewrite.

### Risks
- Existing tests and the startup spec currently codify `deposit` as unknown/out of scope, so the next phases must update specs and tests deliberately rather than layering behavior on top of contradictory expectations.
- `Program.Run` currently owns wallet creation and command dispatch directly; adding deposit may tempt more presentation-level orchestration unless the proposal/design explicitly pulls that flow toward Application.
- Current prompt/output text differs from `TASK.md` examples (`Please enter a command:` vs `Please, submit action:` and a welcome banner exists), so formatting scope must be clarified in later phases.

### Ready for Proposal
Yes — tell the user the codebase is ready for a `money-deposit` proposal, but the proposal/spec must explicitly replace the current startup assumption that `deposit` is an unknown command.
