
namespace RegexRename
{
    using System.Collections.Generic;

    public class AppSettings
    {
        public string DefaultProfileName { get; set; }
        public IDictionary<string, RenameSettings> Profiles { get; }

        public AppSettings()
        {
            Profiles = new Dictionary<string, RenameSettings>();
        }
    }
}
