namespace BetBetBet.Presentation;

public sealed class SystemConsole : IConsole
{
    public void WriteLine(string value) => Console.WriteLine(value);

    public string? ReadLine() => Console.ReadLine();
}
