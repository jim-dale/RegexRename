
namespace RegexRename
{
    using System;
    using System.Collections.Generic;

    public class CommandLine
    {
        private enum ParseState
        {
            ExpectOption,
            ExpectBaseFolder,
            ExpectConfigFileName,
            ExpectProfileName,
            ExpectSourceRegex,
            ExpectDestRegex,
            ExpectVariableTypes
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Rename files using regular expressions.");
            Console.WriteLine();
            Console.WriteLine("  -?        Display this help information.");
            Console.WriteLine("  -c File   Specify the configuration settings file name.");
            Console.WriteLine("  -p Name   Specify profile name to apply to the rename operation.");
            Console.WriteLine("  -l[+]     List profiles.");
            Console.WriteLine("  -r[+|-]   Recurse directories from the specified base directory. Overrides the value from the specified profile.");
            Console.WriteLine("  -w        Displays a message that describes the effect of the command, instead of executing the command.");
            Console.WriteLine("  -x        Displays an example configuration settings file.");
        }

        public static AppContext Parse(string[] args)
        {
            AppContext result = new AppContext();

            ParseState state = ParseState.ExpectOption;
            foreach (var arg in args)
            {
                switch (state)
                {
                    case ParseState.ExpectOption:
                        if (arg.Length > 1 && (arg[0] == '-' || arg[0] == '/'))
                        {
                            switch (Char.ToLowerInvariant(arg[1]))
                            {
                                case '?':
                                    result.ShowHelp = true;
                                    break;
                                case 'b':
                                    state = ParseState.ExpectBaseFolder;
                                    break;
                                case 'c':
                                    state = ParseState.ExpectConfigFileName;
                                    break;
                                case 'p':
                                    state = ParseState.ExpectProfileName;
                                    break;
                                case 'l':
                                    if (arg.Length == 2)
                                    {
                                        result.ListShortProfiles = true;
                                    }
                                    else if (arg.Length > 2)
                                    {
                                        var argOption = arg[2];
                                        if (argOption == '+')
                                        {
                                            result.ListLongProfiles = true;
                                        }
                                    }
                                    break;
                                case 'r':
                                    if (arg.Length == 2)
                                    {
                                        result.Recurse = true;
                                    }
                                    else if (arg.Length > 2)
                                    {
                                        var argOption = arg[2];
                                        if (argOption == '-')
                                        {
                                            result.Recurse = false;
                                        }
                                        else if (argOption == '+')
                                        {
                                            result.Recurse = true;
                                        }
                                    }
                                    break;
                                case 's':
                                    state = ParseState.ExpectSourceRegex;
                                    break;
                                case 'd':
                                    state = ParseState.ExpectDestRegex;
                                    break;
                                case 'v':
                                    state = ParseState.ExpectVariableTypes;
                                    break;
                                case 'w':
                                    result.WhatIf = true;
                                    break;
                                case 'x':
                                    result.ShowExampleSettings = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ParseState.ExpectBaseFolder:
                        result.BaseFolder = arg;
                        state = ParseState.ExpectOption;
                        break;
                    case ParseState.ExpectConfigFileName:
                        result.ConfigFileName = arg;
                        state = ParseState.ExpectOption;
                        break;
                    case ParseState.ExpectProfileName:
                        result.ProfileName = arg;
                        state = ParseState.ExpectOption;
                        break;
                    case ParseState.ExpectSourceRegex:
                        result.SourceRegex = arg;
                        state = ParseState.ExpectOption;
                        break;
                    case ParseState.ExpectDestRegex:
                        result.DestRegex = arg;
                        state = ParseState.ExpectOption;
                        break;
                    case ParseState.ExpectVariableTypes:
                        result.VariableTypes = ConvertStringToNameValueDictionary(arg);
                        state = ParseState.ExpectOption;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        // Expected format name,type,name,type,...
        private static IDictionary<string, string> ConvertStringToNameValueDictionary(string variableTypes)
        {
            IDictionary<string, string> result = null;

            var tempResult = new Dictionary<string, string>();
            if (String.IsNullOrEmpty(variableTypes) == false)
            {
                string[] parts = variableTypes.Split(Constants.VariableTypesSeparator);
                if (parts.Length > 0 && (parts.Length % 2) == 0)
                {
                    for (int i = 0; i < parts.Length; i += 2)
                    {
                        tempResult.Add(parts[i], parts[i + 1]);
                    }
                }
            }
            if (tempResult.Count > 0)
            {
                result = tempResult;
            }
            return result;
        }
    }
}
