using BetBetBet.Presentation;

namespace BetBetBet.Tests.Presentation;

public sealed class FakeConsole : IConsole
{
    private readonly Queue<string?> _inputs;
    public List<string> Outputs { get; } = new();
    public int ReadCount { get; private set; }

    public FakeConsole(params string?[] inputs)
    {
        _inputs = new Queue<string?>(inputs);
    }

    public void WriteLine(string value) => Outputs.Add(value);

    public string? ReadLine()
    {
        ReadCount++;
        return _inputs.Count > 0 ? _inputs.Dequeue() : null;
    }
}
