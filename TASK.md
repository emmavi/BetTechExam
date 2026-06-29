# Technical Assessment Task

We are building a fully regulated online casino for the North American market that focuses on female gamblers. Besides super smooth and modern UI/UX, Our competitive advantage will come in the form of real-time in-game bonusing and live-ops/gamification. To that aim, the underlying casino platform will be built in-house.

In order to achieve our goals we're looking to hire the best engineers who will help us build a top-notch technical solution. If you are excited about highly available and horizontally scalable services, real-time data processing, and sub-10-ms response times, this job is right for you.

Our task is created to examine your problem-solving skills, the quality of your code, and the architectural decisions that you will make. Think about the design, extensibility, configurability, testability and more, without jumping over the line of overengineering. It is going to be used as a foundation for a technical discussion if you proceed to the next stage, so be prepared to explain why certain decisions have been made.

## The problem

Build a console application in .NET Core which mimics the operations of the player wallet that powers our gaming experience.

## Requirements

The wallet is the heart of the gaming experience and as such, it needs to provide the following features:

- In the beginning, the player starts with a balance of $0 and after every operation, their new balance is displayed.
- **Money deposit** — the player must be able to deposit funds.
- **Money withdrawal** — the player must be able to withdraw funds.
- **Placing bets & accepting wins** — the player must be able to play a simple game that simulates a real slot game (see game rules below).
- **Game rules** — our game of chance provides a simple betting experience with the following rules:
  - The game accepts bets between $1 and $10.
  - The game plays out as follows:
    - 50% of the bets lose.
    - 40% of the bets win up to x2 the bet amount.
    - 10% of the bets win between x2 and x10 the bet amount.
  - After every round the player balance is calculated as follows:

    ```
    {new balance} = {old balance} - {bet amount} + {win amount}
    ```
- The game ends when the player decides to leave.

## Important note

All operations that require an amount must include the amount as a positive number, regardless of the direction of the balance.

## Example presentation

```
Please, submit action:
deposit 10
Your deposit of $10 was successful. Your current balance is: $10

Please, submit action:
bet 5
Congrats - you won $35.35! Your current balance is: $40.35

Please, submit action:
bet 10
Congrats - you won $11.90! Your current balance is: $42.25

Please, submit action:
bet 10
Congrats - you won $16.60! Your current balance is: $48.85

Please, submit action:
bet 10
Congrats - you won $15.30! Your current balance is: $54.15

Please, submit action:
bet 10
No luck this time! Your current balance is: $44.15

Please, submit action:
withdraw 44.00
Your withdrawal of $44.00 was successful. Your current balance is: $0.15

Please, submit action:
exit
Thank you for playing! Hope to see you again soon.

Press any key to exit.
```

Good luck and hope to speak to you soon!
