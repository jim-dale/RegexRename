
namespace RegexRename
{
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;

    public static class AppSettingsExtensions
    {
        public const string DefaultFileName = "RegexRename.AppSettings.json";

        public static AppSettings TryDefaultJsonFile(this AppSettings appSettings)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string path = Path.Combine(assemblyPath, DefaultFileName);

            return TryJsonFile(appSettings, path);
        }

        public static AppSettings TryJsonFile(this AppSettings appSettings, string path)
        {
            if (string.IsNullOrEmpty(path) == false && File.Exists(path))
            {
                string json = File.ReadAllText(path);
                JsonConvert.PopulateObject(json, appSettings);
            }
            return appSettings;
        }

        public static void SaveToFile(this AppSettings appSettings, string path)
        {
            string json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
