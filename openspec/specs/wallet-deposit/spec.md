# Wallet Deposit Specification

## Purpose

Define validated local wallet deposits for a single console session.

## Requirements

### Requirement: Successful Deposit

The system MUST accept `deposit <amount>` when `<amount>` is a positive decimal parsed with invariant-culture rules, and MUST add that amount to the session wallet balance.

#### Scenario: Deposit whole amount

- GIVEN the session wallet balance is `$0`
- WHEN the user enters `deposit 10`
- THEN the wallet balance MUST become `$10`
- AND the command loop MUST remain active

#### Scenario: Deposit decimal amount

- GIVEN the session wallet balance is `$10`
- WHEN the user enters `deposit 10.50`
- THEN the wallet balance MUST become `$20.50`
- AND the command loop MUST remain active

### Requirement: Deposit Validation

The system MUST reject missing, invalid, zero, and negative deposit amounts with an error message and MUST NOT change the wallet balance.

#### Scenario: Missing amount is rejected

- GIVEN the session wallet balance is `$0`
- WHEN the user enters `deposit`
- THEN the system MUST print an error message
- AND the wallet balance MUST remain `$0`

#### Scenario: Invalid amount is rejected

- GIVEN the session wallet balance is `$0`
- WHEN the user enters `deposit abc`
- THEN the system MUST print an error message
- AND the wallet balance MUST remain `$0`

#### Scenario: Non-positive amount is rejected

- GIVEN the session wallet balance is `$0`
- WHEN the user enters `deposit 0` or `deposit -1`
- THEN the system MUST print an error message
- AND the wallet balance MUST remain `$0`

### Requirement: Deposit Result Display

The system MUST display the wallet's updated balance after a successful deposit using the exact format: `Your deposit of $X was successful. Your current balance is: $Y`.

#### Scenario: Updated balance is shown

- GIVEN the session wallet balance is `$0`
- WHEN the user enters `deposit 10`
- THEN the console output MUST be `Your deposit of $10 was successful. Your current balance is: $10`

#### Scenario: Rejected deposit does not display a new balance

- GIVEN the session wallet balance is `$0`
- WHEN the user enters `deposit 0`
- THEN the system MUST print an error message
- AND the displayed balance MUST NOT indicate a successful deposit
