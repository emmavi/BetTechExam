# Apply Progress: wallet-initialization

## Status
All tasks complete. 22/22 tests passing. Build zero warnings.

## Completed Tasks
- [x] 1.1 Create `BetBetBet.Domain/Common/Result.cs`
- [x] 1.2 Create `BetBetBet.Domain/Common/Error.cs`
- [x] 1.3 Create `BetBetBet.Domain/ValueObjects/Money.cs`
- [x] 1.4 Create `BetBetBet.Domain/Entities/Wallet.cs`
- [x] 2.1 Create `BetBetBet.Application/Commands/ICommand.cs`
- [x] 2.2 Create `BetBetBet.Application/Commands/ExitCommand.cs`
- [x] 3.1 Create `BetBetBet.Presentation/Parsing/ICommandParser.cs`
- [x] 3.2 Create `BetBetBet.Presentation/Parsing/ExitCommandParser.cs`
- [x] 3.3 Create `BetBetBet.Presentation/Parsing/CommandParserRegistry.cs`
- [x] 3.4 Create `BetBetBet.Presentation/Program.cs`
- [x] 4.1 Add test NuGet packages to `BetBetBet.Tests.csproj`
- [x] 5.1 Create `BetBetBet.Tests/Domain/MoneyTests.cs`
- [x] 5.2 Create `BetBetBet.Tests/Domain/WalletTests.cs`
- [x] 5.3 Create `BetBetBet.Tests/Presentation/ExitCommandParserTests.cs`
- [x] 5.4 Create `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs`
- [x] 6.1 Build verification: `dotnet build BetBetBet\BetBetBet.slnx` — 0 errors, 0 warnings
- [x] 6.2 Test verification: `dotnet test BetBetBet\BetBetBet.slnx` — 18 passed, 0 failed
- [x] 6.3 Manual smoke test attempted; console piping unreliable in headless env; logic verified by tests
- [x] 7.1 Create `BetBetBet.Presentation/IConsole.cs` — abstraction for console I/O
- [x] 7.2 Create `BetBetBet.Presentation/SystemConsole.cs` — production implementation wrapping `System.Console`
- [x] 7.3 Refactor `Program.cs` — extracted static `Run(IConsole)` method; `Main` delegates to `Run(new SystemConsole())`
- [x] 7.4 Create `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` — 4 tests covering startup, exit, unknown, empty input
- [x] 7.5 Test: exit prints goodbye and stops reading (single prompt verified via FakeConsole read counter)
- [x] 7.6 Test: unknown command prints `Unknown command.` and loop continues
- [x] 7.7 Test: empty input prints `Unknown command.` and loop continues
- [x] 7.8 Build and test verification: `dotnet build` 0 errors/0 warnings; `dotnet test` 22 passed, 0 failed

## Mode
Standard (strict TDD disabled per config)

## Files Changed
| File | Action | What Was Done |
|------|--------|---------------|
| `BetBetBet.Domain/Common/Result.cs` | Created | Generic Result<T> with MemberNotNullWhen, implicit conversions |
| `BetBetBet.Domain/Common/Error.cs` | Created | record Error(string Code, string Message) |
| `BetBetBet.Domain/Errors/InputErrors.cs` | Created | Static error catalog: UnknownCommand, InvalidFormat |
| `BetBetBet.Domain/ValueObjects/Money.cs` | Created | Money value object with Create, operators, IComparable |
| `BetBetBet.Domain/Entities/Wallet.cs` | Created | Wallet entity with Balance property |
| `BetBetBet.Application/Commands/ICommand.cs` | Created | Marker interface for parsed commands |
| `BetBetBet.Application/Commands/ExitCommand.cs` | Created | sealed record ExitCommand : ICommand |
| `BetBetBet.Presentation/Parsing/ICommandParser.cs` | Created | Parser interface with Keyword and Parse |
| `BetBetBet.Presentation/Parsing/ExitCommandParser.cs` | Created | Parses "exit", rejects extra args |
| `BetBetBet.Presentation/Parsing/CommandParserRegistry.cs` | Created | Case-insensitive routing; empty/unknown -> UnknownCommand |
| `BetBetBet.Presentation/Program.cs` | Created / Modified | Composition root: create wallet, print balance, run read-dispatch loop; refactored to static class with `Run(IConsole)` |
| `BetBetBet.Presentation/IConsole.cs` | Created | Console abstraction: WriteLine, ReadLine |
| `BetBetBet.Presentation/SystemConsole.cs` | Created | Production IConsole implementation wrapping System.Console |
| `BetBetBet.Tests/BetBetBet.Tests.csproj` | Modified | Added xUnit, FluentAssertions, NSubstitute, Presentation ref |
| `BetBetBet.Tests/Domain/MoneyTests.cs` | Created | 9 tests: Create, equality, arithmetic, comparison |
| `BetBetBet.Tests/Domain/WalletTests.cs` | Created | 2 tests: zero and non-zero balance construction |
| `BetBetBet.Tests/Presentation/ExitCommandParserTests.cs` | Created | 2 tests: parse exit, reject args |
| `BetBetBet.Tests/Presentation/CommandParserRegistryTests.cs` | Created | 5 tests: exit routing, case-insensitive, unknown, empty, null |
| `BetBetBet.Tests/Presentation/SessionRunnerTests.cs` | Created | 4 tests: startup display, exit behavior, unknown command, empty input |
| `BetBetBet.Infra/BetBetBet.Infra.csproj` | Modified | Removed `<OutputType>Exe</OutputType>` to fix build |

## Deviations from Design
None — implementation matches design.

## Issues Found
1. **Infra csproj had `<OutputType>Exe</OutputType>` with no Program.cs**, causing build failure CS5001. Fixed by removing the OutputType tag so Infra is a class library.
2. **Presentation parsing files initially missing `using BetBetBet.Application.Commands;`** — caught by compiler and fixed immediately.
3. **Test project lacked reference to `BetBetBet.Presentation`** — added as part of task 4.1.
4. **Top-level statements in Program.cs blocked testability** — converted to static class with `Run(IConsole)` method. No functional change; entry point preserved.

## Remaining Tasks
None. All phases complete.

## Workload / PR Boundary
- Mode: size:exception (maintainer-approved)
- Current work unit: wallet-initialization (full slice)
- Boundary: entire change from scaffold to runnable wallet session with testable console seam
- Estimated review budget impact: ~470–570 lines as forecasted; approved as single PR

## Next Recommended
sdd-verify
