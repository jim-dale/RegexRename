namespace RegexRename;

using RegexRename.Models;
using RegexRename.Support;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(builder =>
            {
                builder.SetBasePath(AppContext.BaseDirectory);
                builder.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                IConfigurationSection section = hostContext.Configuration.GetSection("Profiles");

                services.Configure<ProfileConfiguration>(section);
                services.AddSingleton(CommandLine.Parse(args, hostContext.Configuration.GetSection("Profiles")));

                services.AddSingleton<ForegroundWorker, Worker>();
            })
            .Build();

        var worker = host.Services.GetRequiredService<ForegroundWorker>();

        return await worker.ExecuteAsync();
    }
}
