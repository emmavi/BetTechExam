# Wallet Withdrawal Specification

## Purpose

Define validated local wallet withdrawals for a single console session.

## Requirements

### Requirement: Successful Withdrawal

The system MUST accept `withdraw <amount>` when `<amount>` is a positive decimal parsed with invariant-culture rules, MUST subtract that amount from available wallet funds, and MUST NOT allow the balance to become negative.

#### Scenario: Withdraw available amount

- GIVEN the session wallet balance is `$20.00`
- WHEN the user enters `withdraw 5`
- THEN the wallet balance MUST become `$15.00`
- AND the command loop MUST remain active

#### Scenario: Successful withdrawal output is shown

- GIVEN the session wallet balance is `$20.00`
- WHEN the user enters `withdraw 5`
- THEN the console output MUST be `Your withdrawal of $5.00 was successful. Your current balance is: $15.00`

### Requirement: Withdrawal Validation and Failures

The system MUST reject malformed, missing, zero, negative, and insufficient-funds withdrawals as expected `Result` failures, MUST NOT throw exceptions for those business outcomes, and MUST leave the wallet balance unchanged.

#### Scenario: Malformed amount follows input error convention

- GIVEN the session wallet balance is `$20.00`
- WHEN the user enters `withdraw abc`
- THEN the system MUST print `Could not parse amount.`
- AND the wallet balance MUST remain `$20.00`

#### Scenario: Non-positive amount is rejected

- GIVEN the session wallet balance is `$20.00`
- WHEN the user enters `withdraw 0` or `withdraw -1`
- THEN the system MUST print `Could not parse amount.`
- AND the wallet balance MUST remain `$20.00`

#### Scenario: Insufficient funds include current balance

- GIVEN the session wallet balance is `$3.00`
- WHEN the user enters `withdraw 5`
- THEN the system MUST print `Insufficient funds for withdrawal. Your current balance is: $3.00`
- AND the wallet balance MUST remain `$3.00`
