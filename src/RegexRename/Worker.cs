namespace RegexRename;

using Microsoft.Extensions.Options;
using RegexRename.Models;
using RegexRename.Support;

public class Worker : ForegroundWorker
{
    private readonly RegexRenameContext context;
    private readonly ProfileConfiguration profileConfiguration;
    private readonly ILogger<Worker> logger;

    public Worker(RegexRenameContext context, IOptions<ProfileConfiguration> options, ILogger<Worker> logger)
    {
        this.context = context;
        this.profileConfiguration = options.Value;
        this.logger = logger;
    }

    public override Task<int> ExecuteAsync()
    {
        if (this.context.Profile != null)
        {
            var profile = this.context.Profile;

            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            logger.LogInformation("Folder: '{Folder}'", profile.Folder);
            logger.LogInformation("FileSearchPattern: '{FileSearchPattern}'", profile.FileSearchPattern);
            logger.LogInformation("SourceRegex: '{SourceRegex}'", profile.InputPattern);
            logger.LogInformation("DestRegex: '{DestRegex}'", profile.OutputPattern);
            logger.LogInformation("WhatIf: {WhatIf}", this.context.WhatIf);

            var source = new InputSource(profile.Folder!, profile.FileSearchPattern!);
            var processor = new FileNameProcessor(profile.InputPattern!, profile.OutputPattern!, profile.VariableTypes!);

            foreach (var inputPath in source.GetFiles())
            {
                logger.LogInformation("File: '{InputPath}'", inputPath);

                string directory = Path.GetDirectoryName(inputPath)!;
                string inputFileName = Path.GetFileName(inputPath);

                var outputFileName = processor.TransformFileName(inputFileName);
                if (string.IsNullOrWhiteSpace(outputFileName) == false)
                {
                    string outputPath = Path.Combine(directory, outputFileName);

                    logger.LogInformation("'{InputPath}' => '{OutputPath}'", inputPath, outputPath);

                    if (context.WhatIf == false)
                    {
                        File.Move(inputPath, outputPath);
                    }
                }
            }
        }
        else
        {
            if (context.ShowHelp)
            {
                CommandLine.ShowHelp();
            }
            if (context.ShowExampleSettings)
            {
                CommandLine.ShowExampleSettings();
            }
            if (context.ListLongProfiles || context.ListShortProfiles)
            {
                CommandLine.ShowProfiles(this.profileConfiguration, context.ListLongProfiles);
            }

            CommandLine.ShowAnyErrors(context.ErrorList);
        }

        return Task.FromResult(context.ExitCode);
    }

}
