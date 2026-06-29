# Wallet Session Startup Specification

## Purpose

Define the first runnable wallet session slice: startup wallet creation, initial balance display, a minimal console input loop, exit handling, and unknown-command handling.

## Requirements

### Requirement: Startup Wallet Balance

The system MUST create a new session wallet whose initial balance is `$0`.

#### Scenario: New wallet starts at zero

- GIVEN the application is starting a new session
- WHEN the session wallet is created
- THEN the wallet balance MUST be `$0`

#### Scenario: Startup balance is not hardcoded separately

- GIVEN a session wallet has been created
- WHEN startup output is produced
- THEN the displayed balance MUST come from the wallet's current balance

### Requirement: Startup Balance Display

The system MUST display the newly created wallet's actual balance before accepting commands.

#### Scenario: Startup displays zero balance

- GIVEN a new session wallet with balance `$0`
- WHEN the application starts
- THEN the console output MUST include the current balance as `$0`

#### Scenario: Startup display occurs before first prompt input

- GIVEN the application has started
- WHEN no user command has been read yet
- THEN the startup balance MUST already have been displayed

### Requirement: Minimal Console Loop

The system MUST read commands from console input repeatedly until the user exits.

#### Scenario: Loop prompts for input

- GIVEN the startup balance has been displayed
- WHEN the command loop begins
- THEN the system MUST request an action from the user

#### Scenario: Loop continues after unsupported input

- GIVEN the command loop is active
- WHEN the user enters a command other than `exit`
- THEN the loop MUST remain active and request another action

### Requirement: Exit Command

The system MUST end the command loop when the user enters `exit` and MUST print `Thank you for playing! Hope to see you again soon.`.

#### Scenario: Exit ends the session

- GIVEN the command loop is active
- WHEN the user enters `exit`
- THEN the system MUST print `Thank you for playing! Hope to see you again soon.`
- AND the command loop MUST stop reading further commands

#### Scenario: Exit is the only supported command

- GIVEN the command loop is active
- WHEN the user enters a command that is not `exit`
- THEN the system MUST NOT treat it as a supported wallet operation

### Requirement: Unknown Command Handling

The system MUST print `Unknown command.` for any non-`exit` input and continue the command loop.

#### Scenario: Unsupported command is rejected

- GIVEN the command loop is active
- WHEN the user enters `deposit 10`
- THEN the system MUST print `Unknown command.`
- AND the loop MUST continue

#### Scenario: Empty input is rejected

- GIVEN the command loop is active
- WHEN the user submits empty input
- THEN the system MUST print `Unknown command.`
- AND the loop MUST continue

### Requirement: Deferred Wallet Operations

The system MUST NOT implement deposit, withdrawal, betting, win handling, game rules, persistence, multi-player state, authentication, or concurrency in this slice.

#### Scenario: Deposit remains out of scope

- GIVEN the command loop is active
- WHEN the user enters `deposit 10`
- THEN the system MUST print `Unknown command.`

#### Scenario: Betting remains out of scope

- GIVEN the command loop is active
- WHEN the user enters `bet 5`
- THEN the system MUST print `Unknown command.`
