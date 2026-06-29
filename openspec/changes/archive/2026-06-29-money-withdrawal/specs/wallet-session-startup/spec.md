# Delta for Wallet Session Startup

## MODIFIED Requirements

### Requirement: Minimal Console Loop

The system MUST read commands from console input repeatedly until the user exits.
(Previously: the loop explicitly covered `deposit` as a supported wallet command but did not cover `withdraw`.)

#### Scenario: Loop prompts for input

- GIVEN the startup balance has been displayed
- WHEN the command loop begins
- THEN the system MUST request an action from the user

#### Scenario: Loop continues after unsupported input

- GIVEN the command loop is active
- WHEN the user enters an unsupported command
- THEN the loop MUST remain active and request another action

#### Scenario: Loop continues after deposit input

- GIVEN the command loop is active
- WHEN the user enters a valid or invalid `deposit` command
- THEN the loop MUST remain active and request another action

#### Scenario: Loop continues after withdrawal input

- GIVEN the command loop is active
- WHEN the user enters a valid or invalid `withdraw` command
- THEN the loop MUST remain active and request another action

### Requirement: Unknown Command Handling

The system MUST print `Unknown command.` for unsupported input that is not `exit`, `deposit`, `withdraw`, or another supported wallet command, then continue the command loop.
(Previously: only `deposit` was explicitly excluded from unknown-command handling.)

#### Scenario: Unsupported command is rejected

- GIVEN the command loop is active
- WHEN the user enters `bet 5`
- THEN the system MUST print `Unknown command.`
- AND the loop MUST continue

#### Scenario: Empty input is rejected

- GIVEN the command loop is active
- WHEN the user submits empty input
- THEN the system MUST print `Unknown command.`
- AND the loop MUST continue

#### Scenario: Supported deposit is not unknown

- GIVEN the command loop is active
- WHEN the user enters `deposit 10`
- THEN the system MUST NOT print `Unknown command.`
- AND the loop MUST continue

#### Scenario: Supported withdrawal is not unknown

- GIVEN the command loop is active
- WHEN the user enters `withdraw 5`
- THEN the system MUST NOT print `Unknown command.`
- AND the loop MUST continue

### Requirement: Deferred Wallet Operations

The system MUST NOT implement betting, win handling, game rules, persistence, multi-player state, authentication, or concurrency in this slice.
(Previously: withdrawal was listed as a deferred wallet operation.)

#### Scenario: Betting remains out of scope

- GIVEN the command loop is active
- WHEN the user enters `bet 5`
- THEN the system MUST print `Unknown command.`

#### Scenario: Generic unsupported command remains rejected

- GIVEN the command loop is active
- WHEN the user enters any unsupported command such as `spin 5`
- THEN the system MUST print `Unknown command.`
