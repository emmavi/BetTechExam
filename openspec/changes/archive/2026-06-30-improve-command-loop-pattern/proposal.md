# Proposal: Improve Command Loop with Presentation Pattern

## Intent

Reduce `Program.Run` complexity by replacing command-specific execution branches with a Presentation-level command execution registry/strategy pattern, while preserving every existing console prompt, output, loop, parser, and wallet behavior.

## Scope

### In Scope
- Introduce Presentation command executors/handlers keyed by parsed command type.
- Keep console/session concerns in Presentation and existing business handlers in Application.
- Simplify `Program.Run` so it composes dependencies, reads input, parses commands, invokes the registry, and applies loop continuation.
- Preserve OpenSpec behavior for startup, unknown input, exit, deposit, withdrawal, and betting.

### Out of Scope
- Application-level `ICommandDispatcher`, mediator, or shared Application result contract; defer to phase 2.
- New wallet commands, changed parsing rules, changed output text, persistence, authentication, multiplayer, or concurrency.

## Capabilities

### New Capabilities
- None

### Modified Capabilities
- None

## Approach

Use the existing parser registry as the model: add a Presentation execution registry and one executor per command (`exit`, `deposit`, `withdraw`, `bet`). Each executor adapts current Application handlers and Presentation formatters to a common Presentation result such as updated wallet plus continue/exit signal. Phase 1 keeps behavior-preserving routing in Presentation. Phase 2 may move dispatch into Application after result shapes and boundaries are deliberately designed.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `BetBetBet/BetBetBet.Presentation/Program.cs` | Modified | Remove command-specific branch chain; keep composition and loop orchestration. |
| `BetBetBet/BetBetBet.Presentation/Commands/` | New | Add Presentation executors/registry/context/result types if needed. |
| `BetBetBet/BetBetBet.Presentation/Parsing/` | Modified | Reuse parsed command contracts without changing parser semantics. |
| `BetBetBet/BetBetBet.Tests/Presentation/` | Modified | Lock exact prompt/output order and command continuation behavior. |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Prompt or output order changes accidentally | Medium | Use existing Presentation/session tests and exact OpenSpec strings. |
| Over-abstracting into Application prematurely | Medium | Keep phase 1 registry Presentation-only. |
| Parser behavior changes while refactoring execution | Low | Do not modify parsing rules except wiring. |

## Rollback Plan

Revert the Presentation registry/executor files and restore the previous `Program.Run` dispatch branches. Because no spec-level behavior or persisted data changes are introduced, rollback is code-only.

## Dependencies

- Existing Application command handlers and Presentation formatters.
- Current OpenSpec specs for wallet session startup, deposit, withdrawal, and betting.

## Success Criteria

- [ ] `Program.Run` no longer contains command-specific execution branching for supported commands.
- [ ] Console prompts, output strings, loop continuation, exit behavior, and wallet updates remain unchanged.
- [ ] Phase 2 Application dispatcher is explicitly deferred, not partially introduced.
