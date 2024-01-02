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
        if (this.context.ShowHelp)
        {
            CommandLine.ShowHelp();
        }
        if (this.context.ShowExampleSettings)
        {
            CommandLine.ShowExampleSettings();
        }
        if (this.context.ListLongProfiles || this.context.ListShortProfiles)
        {
            CommandLine.ShowProfiles(this.profileConfiguration, this.context.ListLongProfiles);
        }

        if (this.context.ShowHelp == false
            && this.context.ShowExampleSettings == false
            && this.context.ListLongProfiles == false
            && this.context.ListShortProfiles == false)
        {
            this.ValidateConfiguration();

            if (CommandLine.HasErrors(this.context))
            {
                CommandLine.ShowHelp();
                CommandLine.ShowAnyErrors(this.context.ErrorList);
            }
            else
            {
                this.ExecuteCore();
            }
        }

        return Task.FromResult(this.context.ExitCode);
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(this.context.Profile.Folder))
        {
            this.context.ErrorList.Add("The folder must be specified.");
        }
        if (string.IsNullOrWhiteSpace(this.context.Profile.FileSearchPattern))
        {
            this.context.ErrorList.Add("The file search pattern must be specified.");
        }
        if (string.IsNullOrWhiteSpace(this.context.Profile.InputPattern))
        {
            this.context.ErrorList.Add("The input file name regular expression pattern must be specified.");
        }
        if (string.IsNullOrWhiteSpace(this.context.Profile.OutputPattern))
        {
            this.context.ErrorList.Add("The output file name format must be specified.");
        }
        if (this.context.Profile.Variables is null || this.context.Profile.Variables.Count == 0)
        {
            this.context.ErrorList.Add("Variables must be defined.");
        }
    }

    private void ExecuteCore()
    {
        var profile = this.context.Profile;
        string folder = Path.GetFullPath(this.context.Profile.Folder!);

        this.logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        this.logger.LogInformation("Folder: '{Folder}'", folder);
        this.logger.LogInformation("FileSearchPattern: '{FileSearchPattern}'", profile.FileSearchPattern);
        this.logger.LogInformation("SourceRegex: '{SourceRegex}'", profile.InputPattern);
        this.logger.LogInformation("DestRegex: '{DestRegex}'", profile.OutputPattern);
        this.logger.LogInformation("WhatIf: {WhatIf}", this.context.WhatIf);

        InputSource source = new(folder, profile.FileSearchPattern!);
        FileNameProcessor processor = new(profile.InputPattern!, profile.OutputPattern!, profile.Variables!);

        foreach (string inputPath in source.GetFiles())
        {
            this.logger.LogInformation("File: '{InputPath}'", inputPath);

            string directory = Path.GetDirectoryName(inputPath)!;
            string inputFileName = Path.GetFileName(inputPath);

            var outputFileName = processor.TransformFileName(inputFileName);
            if (string.IsNullOrWhiteSpace(outputFileName) == false)
            {
                string outputPath = Path.Combine(directory, outputFileName);

                this.logger.LogInformation("'{InputPath}' => '{OutputPath}'", inputPath, outputPath);

                if (this.context.WhatIf == false)
                {
                    File.Move(inputPath, outputPath);
                }
            }
        }
    }
}
