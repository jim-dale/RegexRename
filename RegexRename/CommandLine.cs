
namespace RegexRename
{
    using System;

    public class CommandLine
    {
        private enum ParseState
        {
            ExpectOption,
            ExpectConfigFileName,
            ExpectProfileName
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Rename files using regular expression.");
            Console.WriteLine();
            Console.WriteLine("  -?        Display this help information.");
            Console.WriteLine("  -c        Specify the configuration file name.");
            Console.WriteLine("  -p        Specify profile name to apply to the file rename operation.");
            Console.WriteLine("  -l        List profiles.");
            Console.WriteLine("  -r[+|-]   Recurse directories from the specified base directory. Overrides the value from the specified profile.");
            Console.WriteLine("  -w        Displays a message that describes the effect of the command, instead of executing the command.");
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
                                case 'c':
                                    state = ParseState.ExpectConfigFileName;
                                    break;
                                case 'p':
                                    state = ParseState.ExpectProfileName;
                                    break;
                                case 'l':
                                    result.ListLongProfiles = true;
                                    break;
                                case 's':
                                    result.ListShortProfiles = true;
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
                                case 'w':
                                    result.WhatIf = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ParseState.ExpectConfigFileName:
                        result.ConfigFileName = arg;
                        state = ParseState.ExpectOption;
                        break;
                    case ParseState.ExpectProfileName:
                        result.ProfileName = arg;
                        state = ParseState.ExpectOption;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
    }
}
