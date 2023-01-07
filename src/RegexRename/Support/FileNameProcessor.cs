namespace RegexRename.Support;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mindplay.Extensions;

public class FileNameProcessor
{
    private readonly Regex inputPattern;
    private readonly string outputPattern;
    private readonly Dictionary<string, Type> typeMap = new();

    public FileNameProcessor(string inputPattern, string outputPattern, IDictionary<string, string> variableTypes)
    {
        this.inputPattern = new Regex(inputPattern);
        this.outputPattern = outputPattern;

        InitialiseTypeMap(variableTypes);
    }

    public string? TransformFileName(string fileName)
    {
        string? result = default;

        var matches = inputPattern.Matches(fileName);
        if (matches.Count == 1)
        {
            var match = matches[0];
            if (match.Success)
            {
                var values = GetValues(match);

                result = outputPattern.Subtitute(values);
            }
        }

        return result;
    }

    private void InitialiseTypeMap(IDictionary<string, string> variableTypes)
    {
        var names = inputPattern.GetGroupNames();
        foreach (var name in names)
        {
            var varType = variableTypes.TryGetValue(name, out string? value) ? Type.GetType(value, true) : typeof(string);
            typeMap.Add(name, varType!);
        }
    }

    private IReadOnlyDictionary<string, object> GetValues(Match match)
    {
        var result = new Dictionary<string, object>();
        for (int i = 0; i < match.Groups.Count; i++)
        {
            string name = inputPattern.GroupNameFromNumber(i);
            object value = Convert.ChangeType(match.Groups[i].Value, typeMap[name]);

            result.Add(name, value);
        }
        return result;
    }
}
