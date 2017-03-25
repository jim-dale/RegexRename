
namespace RegexRename
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class AppContext
    {
        public bool ShowHelp { get; set; }
        public bool ShowExampleSettings { get; set; }
        public int Result { get; set; }
        public bool ListLongProfiles { get; set; }
        public bool ListShortProfiles { get; set; }
        public string ConfigFileName { get; set; }
        public string ProfileName { get; set; }
        public bool WhatIf { get; set; }
        public string BaseFolder { get; set; }
        public string FileSearchPattern { get; set; }
        public bool? Recurse { get; set; }
        public string SourceRegex { get; set; }
        public string DestRegex { get; set; }
        public IDictionary<string, string> VariableTypes { get; set; }

        public IList<String> ErrorList { get; }

        public AppSettings Settings { get; private set; }

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

        public void Initialise()
        {
            Settings = new AppSettings()
                .TryDefaultJsonFile()
                .TryJsonFile(ConfigFileName);

            if (string.IsNullOrEmpty(ProfileName))
            {
                ProfileName = Settings.DefaultProfileName;
            }
            var profile = TryGetNamedProfile(ProfileName);

            BaseFolder = MergeStringSetting(BaseFolder, profile?.BaseFolder, Directory.GetCurrentDirectory());
            FileSearchPattern = MergeStringSetting(FileSearchPattern, profile?.FileSearchPattern, Constants.DefaultFileSearchPattern);
            Recurse = MergeBoolSetting(Recurse, profile?.Recurse, Constants.DefaultRecurseFlag);

            SourceRegex = MergeStringSetting(SourceRegex, profile?.SourceRegex, Constants.DefaultSourceRegex);
            DestRegex = MergeStringSetting(DestRegex, profile?.DestRegex, Constants.DefaultDestRegex);
            VariableTypes = MergeVariableTypesSetting(VariableTypes, profile?.VariableTypes);

            if (String.IsNullOrEmpty(BaseFolder) == false)
            {
                BaseFolder = Path.GetFullPath(BaseFolder);
            }

            ValidateContext();
        }

        public bool IsValid
        {
            get
            {
                return ErrorList.Count == 0;
            }
        }

        public RenameContext CreateRenameContext()
        {
            return RenameContext.Create(SourceRegex, DestRegex, VariableTypes);
        }

        public FileSource CreateFileSource()
        {
            var result = new FileSource
            {
                BaseFolder = BaseFolder,
                SearchPattern = FileSearchPattern,
                SearchOption = (Recurse.Value) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
            };
            return result;
        }

        private RenameProfile TryGetNamedProfile(string profileName)
        {
            RenameProfile result = null;
            if (profileName != null)
            {
                result = Settings.Profiles.FirstOrDefault(p => String.Compare(p.Name, profileName, true) == 0);
            }
            return result;
        }

        private void ValidateContext()
        {
            if (String.IsNullOrWhiteSpace(SourceRegex))
            {
                ErrorList.Add("The SourceRegex cannot be empty.");
            }
            if (String.IsNullOrWhiteSpace(DestRegex))
            {
                ErrorList.Add("The DestRegex cannot be empty.");
            }
        }

        private static string MergeStringSetting(string fromCommandLine, string fromProfile, string defaultValue)
        {
            string result = fromCommandLine;
            if (String.IsNullOrEmpty(result))
            {
                result = fromProfile;
            }
            if (String.IsNullOrEmpty(result))
            {
                result = defaultValue;
            }
            return result;
        }

        private static bool MergeBoolSetting(bool? fromCommandLine, bool? fromProfile, bool defaultValue)
        {
            bool? result = fromCommandLine;
            if (result.HasValue == false)
            {
                result = fromProfile;
            }
            if (result.HasValue == false)
            {
                result = defaultValue;
            }
            return result.Value;
        }

        private static IDictionary<string, string> MergeVariableTypesSetting(IDictionary<string, string> fromCommandLine, IDictionary<string, string> fromProfile)
        {
            IDictionary<string, string> result = fromCommandLine;
            if (result == null)
            {
                result = fromProfile;
            }
            return result;
        }
    }
}
