# Delta for Wallet Session Startup

## MODIFIED Requirements

### Requirement: Minimal Console Loop

The system MUST read commands from console input repeatedly until the user exits.
(Previously: any command other than `exit` was treated only as unsupported input.)

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

### Requirement: Exit Command

The system MUST end the command loop when the user enters `exit` and MUST print `Thank you for playing! Hope to see you again soon.`.
(Previously: `exit` was specified as the only supported command.)

#### Scenario: Exit ends the session

- GIVEN the command loop is active
- WHEN the user enters `exit`
- THEN the system MUST print `Thank you for playing! Hope to see you again soon.`
- AND the command loop MUST stop reading further commands

#### Scenario: Exit does not handle supported wallet operations

- GIVEN the command loop is active
- WHEN the user enters `deposit 10`
- THEN the system MUST NOT treat it as `exit`
- AND the command loop MUST remain active

### Requirement: Unknown Command Handling

The system MUST print `Unknown command.` for unsupported input that is not `exit` and not a supported wallet command, then continue the command loop.
(Previously: `deposit 10` was an unsupported command that printed `Unknown command.`.)

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

### Requirement: Deferred Wallet Operations

The system MUST NOT implement withdrawal, betting, win handling, game rules, persistence, multi-player state, authentication, or concurrency in this slice.
(Previously: deposit was listed as a deferred wallet operation.)

#### Scenario: Betting remains out of scope

- GIVEN the command loop is active
- WHEN the user enters `bet 5`
- THEN the system MUST print `Unknown command.`

#### Scenario: Generic unsupported command remains rejected

- GIVEN the command loop is active
- WHEN the user enters any unsupported command such as `withdraw 5`
- THEN the system MUST print `Unknown command.`
