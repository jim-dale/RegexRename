
namespace RegexRename
{
    using System.Collections.Generic;

    public class AppSettings
    {
        public string DefaultProfileName { get; set; }
        public IList<RenameProfile> Profiles { get; set; }
    }
}
