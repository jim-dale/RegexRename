
namespace RegexRename
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Mindplay.Extensions;

    public class RenameContext
    {
        private Regex _regex;
        private string _destRegex;
        private Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();

        public static RenameContext Create(string sourceRegex, string destRegex, IDictionary<string, string> variableTypes)
        {
            var result = new RenameContext();

            result._regex = new Regex(sourceRegex);
            result._destRegex = destRegex;
            result.InitialiseTypeMap(variableTypes);

            return result;
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

                    result = _destRegex.Subtitute(values);
                }
            }
            return result;
        }

        private void InitialiseTypeMap(IDictionary<string, string> variableTypes)
        {
            var names = _regex.GetGroupNames();
            foreach (var name in names)
            {
                var varType = typeof(System.String);
                if (variableTypes != null && variableTypes.ContainsKey(name))
                {
                    varType = Type.GetType(variableTypes[name], true);
                }
                _typeMap.Add(name, varType);
            }
        }

        private IReadOnlyDictionary<String, Object> GetValues(Match match)
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
