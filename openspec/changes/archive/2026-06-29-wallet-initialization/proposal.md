# Proposal: Wallet Initialization

## Intent

Establish the first runnable console-wallet slice: create a real session wallet with `$0`, display that actual balance, then accept minimal commands until exit.

## Scope

### In Scope
- Create a domain-backed `Wallet` initialized with `$0`.
- Display the newly created wallet's actual startup balance.
- Add a minimal `Console.ReadLine` loop.
- Support `exit` and print the required goodbye message.
- For any non-`exit` input, print `Unknown command.` and continue.

### Out of Scope
- Money deposit, money withdrawal, bets, wins, and game rules.
- Full parser registry beyond `exit` and unknown commands.
- Persistence, multi-player state, authentication, and concurrency.

## Capabilities

### New Capabilities
- `wallet-session-startup`: Defines startup wallet creation, initial balance display, minimal console loop behavior, exit handling, and unknown-command handling.

### Modified Capabilities
- None.

## Approach

Use the README Clean Architecture direction without overbuilding. Add the minimal Domain model for a non-negative wallet balance, compose one wallet in Presentation startup, print its balance, then run a simple loop that recognizes `exit`; all other input prints `Unknown command.`. Keep the structure ready for later expansion while deferring deposit, withdrawal, and betting.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `BetBetBet/BetBetBet.Domain/` | New | Minimal wallet/balance model. |
| `BetBetBet/BetBetBet.Presentation/Program.cs` | New/Modified | Startup balance output, read loop, exit, unknown command. |
| `BetBetBet/BetBetBet.Tests/` | New/Modified | Wallet initialization and console-session tests, if test setup is added. |
| `openspec/specs/wallet-session-startup/spec.md` | New | Capability spec from specs phase. |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Slice grows into full command infrastructure too early. | Med | Keep only `exit` and unknown-command behavior in scope. |
| UI hardcodes `$0` instead of reading wallet state. | Med | Specs/tests assert displayed balance comes from wallet. |
| Test project lacks packages. | Med | Add minimum test setup only if needed. |

## Rollback Plan

Revert this change folder and implementation commits touching Domain, Presentation, and Tests. The repo returns to scaffold-only behavior.

## Dependencies

- Existing `.NET 10` solution and Clean Architecture layout.
- Planned test packages if automated tests are implemented.

## Success Criteria

- [ ] App startup creates a real wallet whose balance is `$0`.
- [ ] Startup displays the wallet's actual balance.
- [ ] `exit` ends the loop and prints `Thank you for playing! Hope to see you again soon.`
- [ ] Any non-`exit` input prints `Unknown command.` and keeps reading.
