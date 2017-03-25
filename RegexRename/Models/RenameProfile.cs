
namespace RegexRename
{
    using System.Collections.Generic;

    public class RenameProfile
    {
        public string Name { get; set; }
        public string BaseFolder { get; set; }
        public string FileSearchPattern { get; set; }
        public bool Recurse { get; set; }
        public string SourceRegex { get; set; }
        public string DestRegex { get; set; }
        public IDictionary<string, string> VariableTypes { get; set; }

        public string ToShortString()
        {
            return StringifyProperty(Name);
        }

        public string ToLongString()
        {
            string name = StringifyProperty(Name);
            string baseFolder = StringifyProperty(BaseFolder);
            string fileSearchPattern = StringifyProperty(FileSearchPattern);
            string sourceRegex = StringifyProperty(SourceRegex);
            string destRegex = StringifyProperty(DestRegex);

            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", name, baseFolder, fileSearchPattern, Recurse, sourceRegex, destRegex);
        }

        private string StringifyProperty(string value)
        {
            string result = (value == null) ? "Not Set" : string.Format("\"{0}\"", value);
            return result;
        }
    }
}
