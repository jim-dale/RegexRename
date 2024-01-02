namespace RegexRename.Tests.Support;

internal class ConsoleOutput : IDisposable
{
    private readonly StringWriter stringWriter = new();
    private readonly TextWriter originalOutput;

    public ConsoleOutput()
    {
        this.originalOutput = Console.Out;
        Console.SetOut(this.stringWriter);
    }

    public string GetOuput()
    {
        return this.stringWriter.ToString();
    }

    public void Dispose()
    {
        Console.SetOut(this.originalOutput);
        this.stringWriter.Dispose();
    }
}
