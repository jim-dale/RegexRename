
namespace RegexRename
{
    using System.Collections.Generic;

    public class RenameSettings
    {
        public string Name { get; set; }
        public string BaseFolder { get; set; }
        public string FileNamePattern { get; set; }
        public bool Recurse { get; set; }
        public string SourceRegex { get; set; }
        public string DestRegex { get; set; }
        public IDictionary<string, string> VariableTypes { get; }

        public RenameSettings()
        {
            FileNamePattern = "*";
            VariableTypes = new Dictionary<string, string>();
        }

        public string ToShortString()
        {
            return string.Format("{0}", Name);
        }

        public string ToLongString()
        {
            string baseFolder = (BaseFolder == null) ? "Not Set" : string.Format("\"{0}\"", BaseFolder);

            return string.Format("{0}, {1}, \"{2}\", {3}, \"{4}\", \"{5}\"", Name, baseFolder, FileNamePattern, Recurse, SourceRegex, DestRegex);
        }
    }
}
