
namespace RegexRename
{
    using System.Collections.Generic;
    using System.IO;

    public class FileSource
    {
        public string BaseFolder { get; set; }
        public string SearchPattern { get; set; } = Constants.DefaultFileSearchPattern;
        public SearchOption SearchOption { get; set; } = SearchOption.TopDirectoryOnly;

        public IEnumerable<string> Files
        {
            get
            {
                return Directory.EnumerateFiles(BaseFolder, SearchPattern, this.SearchOption);
            }
        }
    }
}
