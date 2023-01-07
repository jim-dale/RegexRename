namespace RegexRename;

using RegexRename.Support;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(CommandLine.Parse(args, hostContext.Configuration.GetSection("Profiles")));

                services.AddSingleton<ForegroundWorker, Worker>();
            })
            .Build();

        var worker = host.Services.GetRequiredService<ForegroundWorker>();

        return await worker.ExecuteAsync();
    }
}
