
using System;
using System.Collections.Generic;
using System.IO;

namespace RegexRename
{
    class Program
    {
        static int Main(string[] args)
        {
            var context = CommandLine.Parse(args);
            if (context.ShowHelp)
            {
                CommandLine.ShowHelp();
            }
            else
            {
                context.Configure();

                if (context.ListLongProfiles || context.ListShortProfiles)
                {
                    ShowProfiles(context);
                }
                else
                {
                    if (context.HasError == false)
                    {
                        var renameContext = context.CreateInitialisedRenameContext();
                        var fileSource = context.CreateInitialisedFileSourceForRenameContext(renameContext);

                        foreach (var file in fileSource.Files)
                        {
                            string directory = Path.GetDirectoryName(file);
                            string fileName = Path.GetFileName(file);

                            string destFileName = renameContext.TransformFileName(fileName);
                            if (String.IsNullOrEmpty(destFileName) == false)
                            {
                                string destFile = Path.Combine(directory, destFileName);

                                Console.WriteLine($"\"{file}\"=>\"{destFileName}\"");

                                if (context.WhatIf == false)
                                {
                                    File.Move(file, destFile);
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowErrors(context.ErrorList);
                    }
                }
            }
            return context.Result;
        }

        private static void ShowErrors(IList<string> errorList)
        {
            foreach (var error in errorList)
            {
                Console.WriteLine(error);
            }
        }

        private static void ShowProfiles(AppContext context)
        {
            foreach (var profile in context.Settings.Profiles)
            {
                string output = (context.ListLongProfiles) ? profile.Value.ToLongString() : profile.Value.ToShortString();

                Console.WriteLine(output);
            }
        }
    }
}
