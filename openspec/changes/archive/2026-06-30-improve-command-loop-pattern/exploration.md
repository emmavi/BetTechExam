## Exploration: Improve command loop with a pattern

### Current State
`Program.Run` in `BetBetBet.Presentation` owns almost the entire interactive flow: startup messages, wallet creation, parser registry creation, infrastructure composition (`SystemRandomProvider` / `SlotGameEngine`), the infinite read loop, command parsing, command dispatch, result handling, wallet state updates, and console formatting. Parsing already uses a small registry/strategy pattern via `CommandParserRegistry` plus one parser per keyword, but execution does not: `Program.Run` branches with command-specific `if` checks for `ExitCommand`, `DepositCommand`, `WithdrawCommand`, and `BetCommand`. That means adding or changing commands currently requires editing the central loop. The README explicitly describes an `ICommandDispatcher` in Application, but the current codebase does not implement it yet.

### Affected Areas
- `BetBetBet/BetBetBet.Presentation/Program.cs` — the refactor target; currently mixes composition root, loop control, dispatch, state mutation, and output formatting.
- `BetBetBet/BetBetBet.Presentation/Parsing/CommandParserRegistry.cs` — current parser registry pattern that can inform a matching execution/dispatch pattern.
- `BetBetBet/BetBetBet.Application/Handlers/DepositCommandHandler.cs` — current command-specific application handler used directly from `Program.Run`.
- `BetBetBet/BetBetBet.Application/Handlers/WithdrawCommandHandler.cs` — same direct coupling from Presentation.
- `BetBetBet/BetBetBet.Application/Handlers/BetCommandHandler.cs` — highlights return-shape differences (`BetResult` vs `Wallet`) that a dispatcher/handler pattern must normalize.
- `BetBetBet/BetBetBet.Application/Commands/*.cs` — command contracts that a registry or dispatcher would route by type.
- `BetBetBet/BetBetBet.Presentation/BetResultFormatter.cs` — existing formatting seam for betting output that should stay out of the loop.
- `BetBetBet/BetBetBet.Tests/Presentation/SessionRunnerTests.cs` — high-value behavior lock for prompt order, continuation, exit behavior, and output text.
- `BetBetBet/BetBetBet.Tests/Presentation/*CommandParserTests.cs` — parser behavior must remain unchanged while loop execution is simplified.
- `openspec/specs/wallet-session-startup/spec.md` — source-of-truth loop behavior to preserve.
- `openspec/specs/wallet-deposit/spec.md` — success/error display and loop-continuation rules to preserve.
- `openspec/specs/wallet-withdrawal/spec.md` — withdrawal output/error conventions to preserve.
- `openspec/specs/wallet-betting/spec.md` — betting result messaging and continuation rules to preserve.

### Approaches
1. **Presentation command-handler registry** — introduce one presentation-facing handler per parsed command (including exit), with a registry keyed by command type. Each handler executes the application handler it needs, updates session state through a shared context, and returns a loop action/output.
   - Pros: removes the growing `if` chain cleanly, reuses the existing registry idea, keeps console-specific output decisions in Presentation, and can be added incrementally without forcing a bigger Application redesign.
   - Cons: introduces a new abstraction layer in Presentation and still leaves composition/manual wiring in `Program.Run` unless a second extraction is done.
   - Effort: Medium

2. **Application-level dispatcher / mediator** — implement the README-promised `ICommandDispatcher` so `Program.Run` only parses input, calls a dispatcher, and renders a normalized response.
   - Pros: best alignment with the documented architecture, centralizes command routing outside Presentation, and creates a stronger extension point for future commands.
   - Cons: broader refactor because current handlers return different result shapes and some formatting/state decisions are still console-specific; easy to over-design for this console app.
   - Effort: High

### Recommendation
Start with a Presentation command-handler registry. It solves the real pain point — the bloated `Program.Run` branch chain — while preserving current behavior and respecting the existing separation where parsing and console formatting already live in Presentation. If the team later wants stricter README alignment, that registry can be a stepping stone toward an Application dispatcher without forcing the bigger redesign into this refactor.

### Risks
- Presentation tests assert exact output order and text, so even a “safe” refactor can break behavior if prompt timing or formatting changes.
- Current command handlers do not share a common result contract, so a simplification pattern must define how wallet updates, exit signals, and formatted messages are normalized.
- `Program.Run` currently acts as both composition root and runtime orchestrator; moving too much at once can blur whether object construction or command execution is being refactored.
- Parser behavior is slightly inconsistent (`exit` rejects extra args explicitly; amount parsers mostly validate only the first arg), so a broader pattern may accidentally “fix” behavior that current tests/specs do not cover.

### Ready for Proposal
Yes — tell the user the repo is ready for a proposal focused on simplifying `Program.Run` with a command execution/registry pattern that preserves all current OpenSpec behaviors for startup, deposit, withdrawal, betting, and exit.
