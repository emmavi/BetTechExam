# Tasks: Wallet Initialization

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~470–570 |
| 400-line budget risk | High |
| Chained PRs recommended | No |
| Suggested split | Single PR |
| Delivery strategy | exception-ok |
| Chain strategy | size-exception |

Decision needed before apply: No
Chained PRs recommended: No
Chain strategy: size-exception
400-line budget risk: High

## Phase 1: Foundation — Domain Primitives

- [x] 1.1 Create `BetBetBet.Domain/Common/Result.cs` — generic `Result<T>` with `IsSuccess`, `Value`, `Error`, `MemberNotNullWhen`, implicit conversions from `T` and `Error`
- [x] 1.2 Create `BetBetBet.Domain/Common/Error.cs` — `record Error(string Code, string Message)`
- [x] 1.3 Create `BetBetBet.Domain/ValueObjects/Money.cs` — private ctor, `Create(decimal) → Result<Money>`, equality, `IComparable<Money>`, `+`/`-` operators
- [x] 1.4 Create `BetBetBet.Domain/Entities/Wallet.cs` — ctor takes `Money`, exposes `Balance` property; deposit/withdraw/betting stubs deferred

## Phase 2: Application Contracts

- [x] 2.1 Create `BetBetBet.Application/Commands/ICommand.cs` — marker interface
- [x] 2.2 Create `BetBetBet.Application/Commands/ExitCommand.cs` — `sealed record ExitCommand : ICommand`

## Phase 3: Presentation — Parsing & Loop

- [x] 3.1 Create `BetBetBet.Presentation/Parsing/ICommandParser.cs` — `string Keyword { get; }`, `Result<ICommand> Parse(string[] args)`
- [x] 3.2 Create `BetBetBet.Presentation/Parsing/ExitCommandParser.cs` — keyword `exit`, returns `ExitCommand`, rejects extra args
- [x] 3.3 Create `BetBetBet.Presentation/Parsing/CommandParserRegistry.cs` — case-insensitive keyword routing; empty input returns `UnknownCommand` error
- [x] 3.4 Create `BetBetBet.Presentation/Program.cs` — create `Wallet` with `Money.Create(0)`, print displayed balance, `Console.ReadLine` loop, `exit` → goodbye message, else `Unknown command.`

## Phase 4: Test Setup — NuGet Dependencies

- [x] 4.1 Add `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `FluentAssertions`, and `NSubstitute` packages to `BetBetBet.Tests.csproj`

## Phase 5: Unit Tests

- [x] 5.1 Create `BetBetBet.Tests/Domain/MoneyTests.cs` — construction succeeds at `$0`, equality, arithmetic, `Create(-1)` returns error
- [x] 5.2 Create `BetBetBet.Tests/Domain/WalletTests.cs` — wallet initialized at `$0`, `Balance` matches constructor value
- [x] 5.3 Create `BetBetBet.Tests/Presentation/ExitCommandParserTests.cs` — parses `exit`, returns `ExitCommand`, rejects extra args
- [x] 5.4 Create `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs` — routes `exit` (case-insensitive), unknown command returns `UnknownCommand`, empty input returns `UnknownCommand`

## Phase 6: Verification

- [x] 6.1 Run `dotnet build BetBetBet\BetBetBet.slnx` — confirm zero compilation errors/warnings
- [x] 6.2 Run `dotnet test BetBetBet\BetBetBet.slnx` — all tests green
- [x] 6.3 Manual smoke test: startup prints balance, `exit` prints goodbye message, empty/unknown input prints `Unknown command.`

## Phase 7: Remediation — Console Testability

- [x] 7.1 Create `BetBetBet.Presentation/IConsole.cs` — interface with `WriteLine(string)`, `ReadLine() → string?`
- [x] 7.2 Create `BetBetBet.Presentation/SystemConsole.cs` — wraps `Console.WriteLine`/`Console.ReadLine`
- [x] 7.3 Refactor `Program.cs` — extract `Run(IConsole)` method with full startup+loop logic; `Main` delegates to `Run(new SystemConsole())`
- [x] 7.4 Create `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` — test startup displays `$0` from wallet balance before first prompt
- [x] 7.5 Add test: `exit` prints `Thank you for playing!` and loop stops reading (no second prompt)
- [x] 7.6 Add test: unknown command `deposit 10` prints `Unknown command.` and loop continues to next prompt
- [x] 7.7 Add test: empty input prints `Unknown command.` and loop continues
- [x] 7.8 Run `dotnet build` and `dotnet test` — build zero errors/warnings, all tests (existing + new) green
