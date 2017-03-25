
namespace RegexRename
{
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;

    public static class AppSettingsExtensions
    {
        public static AppSettings TryDefaultJsonFile(this AppSettings appSettings)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string path = Path.Combine(assemblyPath, Constants.DefaultSettingsFileName);

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
            string json = appSettings.GetJson();
            File.WriteAllText(path, json);
        }

        public static string GetJson(this AppSettings appSettings)
        {
            return JsonConvert.SerializeObject(appSettings, Formatting.Indented);
        }
    }
}
