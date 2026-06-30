# Wallet Betting Specification

## Purpose

Define bet validation, game outcome tiers, wallet settlement, and user-visible betting results for a single-session wallet game.

## Requirements

### Requirement: Bet Amount Validation

The system MUST accept only bet amounts from `$1` through `$10`, parsed using invariant culture.

#### Scenario: Minimum and maximum bets are accepted

- GIVEN a player has sufficient balance
- WHEN the player bets `$1` or `$10`
- THEN the bet MUST be accepted for game play

#### Scenario: Out-of-range bet is rejected

- GIVEN a player enters a bet below `$1` or above `$10`
- WHEN the bet is handled
- THEN the system MUST return an out-of-range business failure
- AND the balance MUST remain unchanged

### Requirement: Sufficient Funds Validation

The system MUST reject a bet when the player balance is lower than the bet amount.

#### Scenario: Insufficient balance rejects bet

- GIVEN a wallet balance of `$4`
- WHEN the player bets `$5`
- THEN the system MUST return an insufficient-funds business outcome
- AND the balance MUST remain `$4`

### Requirement: Game Outcome Tiers

The system MUST classify bets so 50% lose, 40% win up to x2 the bet amount, and 10% win between x2 and x10 the bet amount.

#### Scenario: Losing tier returns no win

- GIVEN deterministic game input in the losing tier
- WHEN a bet is played
- THEN the win amount MUST be `$0`

#### Scenario: Winning tiers return bounded wins

- GIVEN deterministic game input in normal or big-win tiers
- WHEN a bet is played
- THEN a normal win MUST be `> $0` and `<= 2x bet`
- AND a big win MUST be `> 2x bet` and `<= 10x bet`

### Requirement: Bet Settlement

The system MUST settle every accepted bet as `new balance = old balance - bet amount + win amount`.

#### Scenario: Loss deducts the bet

- GIVEN a wallet balance of `$10`
- WHEN a `$5` bet loses
- THEN the new balance MUST be `$5`

#### Scenario: Win deducts bet and adds win

- GIVEN a wallet balance of `$10`
- WHEN a `$5` bet wins `$8`
- THEN the new balance MUST be `$13`

### Requirement: Betting Output

The system MUST print exact betting result messages and continue the session after handled bets.

#### Scenario: Winning bet prints win message

- GIVEN an accepted bet wins `$8`
- WHEN the result is displayed
- THEN output MUST include `Congrats - you won $8.00! Your current balance is: $13.00`

#### Scenario: Losing bet prints loss message

- GIVEN an accepted bet loses
- WHEN the result is displayed
- THEN output MUST include `No luck this time! Your current balance is: $5.00`
