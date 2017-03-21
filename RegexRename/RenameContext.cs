
namespace RegexRename
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using Mindplay.Extensions;

    public class RenameContext
    {
        private Regex _regex;
        private Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();
        public RenameSettings Settings { get; }
        public string BaseFolder { get; set; }

        public RenameContext(RenameSettings settings)
        {
            Settings = settings;
        }

        public void Initialise()
        {
            _regex = new Regex(Settings.SourceRegex);

            var names = _regex.GetGroupNames();
            foreach (var name in names)
            {
                var varType = typeof(System.String);
                if (Settings.VariableTypes.ContainsKey(name))
                {
                    varType = Type.GetType(Settings.VariableTypes[name], true);
                }
                _typeMap.Add(name, varType);
            }
            if (String.IsNullOrEmpty(BaseFolder))
            {
                BaseFolder = Directory.GetCurrentDirectory();
            }
            BaseFolder = Path.GetFullPath(BaseFolder);
        }

        public string TransformFileName(string fileName)
        {
            string result = null;

            var matches = _regex.Matches(fileName);
            if (matches.Count == 1)
            {
                var match = matches[0];
                if (match.Success)
                {
                    var values = GetValues(match);

                    result = Settings.DestRegex.Subtitute(values);
                }
            }
            return result;
        }

        public IReadOnlyDictionary<String, Object> GetValues(Match match)
        {
            var result = new Dictionary<string, object>();
            for (int i = 0; i < match.Groups.Count; i++)
            {
                string name = _regex.GroupNameFromNumber(i);
                object value = Convert.ChangeType(match.Groups[i].Value, _typeMap[name]);

                result.Add(name, value);
            }
            return result;
        }
    }
}
