namespace RegexRename.Support;

using System;
using System.Diagnostics.CodeAnalysis;
using RegexRename.Models;

public static class CommandLine
{
    private enum ParseState
    {
        ExpectOption,
        ExpectProfileName,
        ExpectFolder,
        ExpectFileSearchPattern,
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
        var result = new RegexRenameContext();

        var profileConfiguration = configurationSection.Get<ProfileConfiguration>();

        var profileNameFomCommandLine = string.Empty;
        var profileFromCommandLine = new Profile();

        ParseState state = ParseState.ExpectOption;

        foreach (var arg in args)
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
                default:
                    break;
            }
        }

        return ValidateConfiguration(result, profileConfiguration, profileNameFomCommandLine, profileFromCommandLine);
    }

    private static RegexRenameContext ValidateConfiguration(RegexRenameContext result, ProfileConfiguration? profileConfiguration, string profileNameFomCommandLine, Profile profileFromCommandLine)
    {
        if (profileConfiguration == null || profileConfiguration.Items.Any() == false)
        {
            result.ErrorList.Add("At least one profile must be configured.");
        }
        else
        {
            if (TryGetProfile(profileNameFomCommandLine, profileConfiguration, out var profile))
            {
                // Merge settings

                profile.Folder = MergeStringSetting(profile.Folder, profileFromCommandLine.Folder, Directory.GetCurrentDirectory());
                profile.FileSearchPattern = MergeStringSetting(profile.FileSearchPattern, profileFromCommandLine.FileSearchPattern, Constants.DefaultFileSearchPattern);

                if (string.IsNullOrWhiteSpace(profile.Folder))
                {
                    result.ErrorList.Add("The folder must be specified.");
                }
                if (string.IsNullOrWhiteSpace(profile.FileSearchPattern))
                {
                    result.ErrorList.Add("The file search pattern must be specified.");
                }
                if (string.IsNullOrWhiteSpace(profile.InputPattern))
                {
                    result.ErrorList.Add("The source file name regular expression must be specified.");
                }
                if (string.IsNullOrWhiteSpace(profile.OutputPattern))
                {
                    result.ErrorList.Add("The destination file name regular expression must be specified.");
                }

                if (result.ErrorList.Count == 0)
                {
                    profile.Folder = Path.GetFullPath(profile.Folder!);
                    result.Profile = profile;
                }
            }
            else
            {
                result.ErrorList.Add("A valid profile must be specified.");
            }
        }
        if (result.ErrorList.Count > 0)
        {
            result.ShowHelp = true;
        }

        return result;
    }

    public static void ShowAnyErrors(IList<string> items)
    {
        if (items.Any())
        {
            Console.WriteLine();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }
    }

    public static void ShowProfiles(ProfileConfiguration configuration, bool isLongListing)
    {
        foreach (var item in configuration.Items)
        {
            string output = isLongListing ? item.Value.ToLongString(item.Key) : item.Value.ToShortString(item.Key);

            Console.WriteLine(output);
        }
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

    private static bool TryGetProfile(string? profileName, ProfileConfiguration profileConfiguration, [NotNullWhen(true)] out Profile? result)
    {
        bool success = false;
        result = default;

        if (string.IsNullOrWhiteSpace(profileName))
        {
            profileName = profileConfiguration.DefaultProfile;
        }
        if (string.IsNullOrWhiteSpace(profileName) == false && profileConfiguration.Items.TryGetValue(profileName, out result))
        {
            success = true;
        }

        return success;
    }
    private static string? MergeStringSetting(string? fromCommandLine, string? fromProfile, string defaultValue)
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
}
