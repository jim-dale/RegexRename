
namespace RegexRename
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class AppContext
    {
        public const string DefaultFileName = "RegexRename.AppSettings.json";

        public bool ShowHelp { get; set; }
        public int Result { get; set; }
        public bool ListLongProfiles { get; set; }
        public bool ListShortProfiles { get; set; }
        public string ConfigFileName { get; set; }
        public string ProfileName { get; set; }
        public bool? Recurse { get; set; }
        public bool WhatIf { get; set; }

        public IList<String> ErrorList { get; }

        public AppSettings Settings { get; private set; }
        public RenameSettings RenameSettings { get; private set; }

        public RenameContext Renamer { get; set; }

        public AppContext()
        {
            Result = 0;
            ListLongProfiles = false;
            ListShortProfiles = false;
            ConfigFileName = String.Empty;
            ProfileName = String.Empty;
            Recurse = null;
            WhatIf = false;

            ErrorList = new List<String>();
        }

        public void Configure()
        {
            Settings = new AppSettings()
                .TryDefaultJsonFile()
                .TryJsonFile(ConfigFileName);

            if (string.IsNullOrEmpty(ProfileName))
            {
                ProfileName = Settings.DefaultProfileName;
            }
            this.RenameSettings = TryGetNamedProfile(ProfileName);

            if (string.IsNullOrEmpty(ProfileName))
            {
                ErrorList.Add("A valid profile name must be specified.");
                Result = 1;
            }
            else if (this.RenameSettings == null)
            {
                ErrorList.Add(String.Format($"The specified profile \"{ProfileName}\" cannot be found."));
                Result = 1;
            }
            else
            {
                if (String.IsNullOrEmpty(this.RenameSettings.SourceRegex))
                {
                    ErrorList.Add(String.Format($"The specified profile \"{ProfileName}\" cannot be found."));
                    Result = 1;
                }
            }
        }

        public bool HasError
        {
            get
            {
                return ErrorList.Count > 0;
            }
        }

        public RenameContext CreateInitialisedRenameContext()
        {
            var result = new RenameContext(this.RenameSettings);
            result.Initialise();
            return result;
        }

        public FileSource CreateInitialisedFileSourceForRenameContext(RenameContext renameContext)
        {
            bool recurse = Recurse ?? renameContext.Settings.Recurse;

            var result = new FileSource
            {
                BaseFolder = renameContext.BaseFolder,
                SearchPattern = renameContext.Settings.FileNamePattern,
                SearchOption = (recurse) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            };
            return result;
        }

        private RenameSettings TryGetNamedProfile(string profileName)
        {
            RenameSettings result = null;

            if (String.IsNullOrEmpty(profileName) == false)
            {
                Settings.Profiles.TryGetValue(profileName, out result);
            }
            return result;
        }
    }
}
