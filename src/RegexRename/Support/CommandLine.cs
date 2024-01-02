namespace RegexRename.Support;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using RegexRename.Models;

public static class CommandLine
{
    private enum ParseState
    {
        ExpectOption,
        ExpectProfileName,
        ExpectFolder,
        ExpectFileSearchPattern,
        ExpectInputPattern,
        ExpectOutputPattern,
        ExpectVariableDeclarations,
    }

    public static void ShowHelp()
    {
        Console.WriteLine("Rename files using regular expressions.");
        Console.WriteLine();
        Console.WriteLine("  -?        Display this help information.");
        Console.WriteLine("  -f Folder Specify the folder to search.");
        Console.WriteLine("  -s Files  Specify the files to match e.g. '*.pdf', '**/*.pdf'.");
        Console.WriteLine("  -p Name   Specify profile name to apply to the rename operation.");
        Console.WriteLine("  -l[+]     List profiles.");
        Console.WriteLine("  -i        Input regular expression pattern to match file name to be processed.");
        Console.WriteLine("  -o        Output format for matched file names.");
        Console.WriteLine("  -d        Variable declarations, name and type, for the output format.");
        Console.WriteLine("  -w        Displays a message that describes the effect of the command, instead of executing the command.");
        Console.WriteLine("  -x        Displays example configuration settings.");
    }

    public static void ShowExampleSettings()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(Example.ProfileConfiguration);
        Console.WriteLine(json);
    }

    public static RegexRenameContext Parse(string[] args, IConfigurationSection configurationSection)
    {
        RegexRenameContext result = new();

        string profileNameFomCommandLine = string.Empty;
        Profile profileFromCommandLine = new();

        ParseState state = ParseState.ExpectOption;

        foreach (string arg in args)
        {
            switch (state)
            {
                case ParseState.ExpectOption:
                    if (arg.Length > 1 && (arg[0] == '-' || arg[0] == '/'))
                    {
                        state = ParseOption(arg, result);
                    }
                    break;
                case ParseState.ExpectFolder:
                    profileFromCommandLine.Folder = arg;
                    state = ParseState.ExpectOption;
                    break;
                case ParseState.ExpectFileSearchPattern:
                    profileFromCommandLine.FileSearchPattern = arg;
                    state = ParseState.ExpectOption;
                    break;
                case ParseState.ExpectProfileName:
                    profileNameFomCommandLine = arg;
                    state = ParseState.ExpectOption;
                    break;
                case ParseState.ExpectInputPattern:
                    profileFromCommandLine.InputPattern = arg;
                    state = ParseState.ExpectOption;
                    break;
                case ParseState.ExpectOutputPattern:
                    profileFromCommandLine.OutputPattern = arg;
                    state = ParseState.ExpectOption;
                    break;
                case ParseState.ExpectVariableDeclarations:
                    profileFromCommandLine.Variables = ParseVariableDeclarations(arg);
                    state = ParseState.ExpectOption;
                    break;
                default:
                    break;
            }
        }

        return SetProfile(result, configurationSection, profileNameFomCommandLine, profileFromCommandLine);
    }

    // Expected format "name,type;name,type;..."
    public static IDictionary<string, string>? ParseVariableDeclarations(string? variableDefinitions)
    {
        if (string.IsNullOrWhiteSpace(variableDefinitions))
        {
            throw new ArgumentException($"'{nameof(variableDefinitions)}' cannot be null or whitespace.", nameof(variableDefinitions));
        }

        Dictionary<string, string> variables = [];

        string[] nameAndTypePairs = variableDefinitions.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string nameAndTypePair in nameAndTypePairs)
        {
            string[] nameAndType = nameAndTypePair.Split(',', 2, StringSplitOptions.TrimEntries);

            variables.Add(nameAndType[0], nameAndType[1]);
        }

        return variables.Count > 0 ? variables : null;
    }

    public static bool HasErrors(RegexRenameContext context)
    {
        return context.ErrorList.Any();
    }

    public static void ShowAnyErrors(IList<string> items)
    {
        if (items.Any())
        {
            Console.WriteLine();
            foreach (string item in items)
            {
                Console.WriteLine(item);
            }
        }
    }

    public static void ShowProfiles(ProfileConfiguration configuration, bool isLongListing)
    {
        foreach (var item in configuration.Items)
        {
            string output = isLongListing
                ? item.Value.ToLongString(item.Key)
                : Profile.ToShortString(item.Key);

            Console.WriteLine(output);
        }
    }

    private static RegexRenameContext SetProfile(RegexRenameContext result, IConfigurationSection configurationSection, string profileNameFomCommandLine, Profile profileFromCommandLine)
    {
        ProfileConfiguration? profileConfiguration = configurationSection.Get<ProfileConfiguration>();

        if (TryGetProfile(profileNameFomCommandLine, profileConfiguration, out var profile) == true)
        {
            result.Profile = profile;
        }

        // Merge settings
        result.Profile.Folder = GetPrioritisedSetting(result.Profile.Folder, profileFromCommandLine.Folder, Directory.GetCurrentDirectory());
        result.Profile.FileSearchPattern = GetPrioritisedSetting(result.Profile.FileSearchPattern, profileFromCommandLine.FileSearchPattern, Constants.DefaultFileSearchPattern);
        result.Profile.InputPattern = GetPrioritisedSetting(result.Profile.InputPattern, profileFromCommandLine.InputPattern, string.Empty);
        result.Profile.OutputPattern = GetPrioritisedSetting(result.Profile.OutputPattern, profileFromCommandLine.OutputPattern, string.Empty);
        result.Profile.Variables = GetPrioritisedSetting(result.Profile.Variables, profileFromCommandLine.Variables);

        return result;
    }

    private static ParseState ParseOption(string arg, RegexRenameContext context)
    {
        var result = ParseState.ExpectOption;

        switch (char.ToUpperInvariant(arg[1]))
        {
            case '?':
                context.ShowHelp = true;
                break;
            case 'F':
                result = ParseState.ExpectFolder;
                break;
            case 'S':
                result = ParseState.ExpectFileSearchPattern;
                break;
            case 'P':
                result = ParseState.ExpectProfileName;
                break;
            case 'L':
                if (arg.Length == 2)
                {
                    context.ListShortProfiles = true;
                }
                else if (arg.Length > 2 && arg[2] == '+')
                {
                    context.ListLongProfiles = true;
                }
                break;
            case 'W':
                context.WhatIf = true;
                break;
            case 'X':
                context.ShowExampleSettings = true;
                break;
            default:
                break;
        }

        return result;
    }

    private static bool TryGetProfile(string? profileName, ProfileConfiguration? profileConfiguration, [NotNullWhen(true)] out Profile? result)
    {
        result = default;
        bool success = false;

        if (profileConfiguration is not null)
        {
            if (string.IsNullOrWhiteSpace(profileName) == false && profileConfiguration.Items.TryGetValue(profileName, out result))
            {
                success = true;
            }
        }

        return success;
    }

    private static string? GetPrioritisedSetting(string? fromCommandLine, string? fromProfile, string defaultValue)
    {
        string? result = fromCommandLine;

        if (string.IsNullOrWhiteSpace(result))
        {
            result = fromProfile;
        }
        if (string.IsNullOrWhiteSpace(result))
        {
            result = defaultValue;
        }

        return result;
    }

    private static IDictionary<string, string>? GetPrioritisedSetting(IDictionary<string, string>? fromCommandLine, IDictionary<string, string>? fromProfile)
    {
        IDictionary<string, string>? result = fromCommandLine;

        if (result is null || result.Count == 0)
        {
            result = fromProfile;
        }

        return result;
    }
}
