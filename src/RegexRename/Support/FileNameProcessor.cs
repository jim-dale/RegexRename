namespace RegexRename.Support;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Mindplay.Extensions;

public class FileNameProcessor
{
    private readonly Regex inputPattern;
    private readonly string outputPattern;
    private readonly Dictionary<string, Type> typeMap = [];

    public FileNameProcessor(string inputPattern, string outputPattern, IDictionary<string, string> variables)
    {
        this.inputPattern = new Regex(inputPattern);
        this.outputPattern = outputPattern;

        this.InitialiseTypeMap(variables);
    }

    public string? TransformFileName(string fileName)
    {
        string? result = default;

        var matches = this.inputPattern.Matches(fileName);
        if (matches.Count == 1)
        {
            var match = matches[0];
            if (match.Success)
            {
                var values = this.GetValues(match);

                result = this.outputPattern.Subtitute(values);
            }
        }

        return result;
    }

    private void InitialiseTypeMap(IDictionary<string, string> variableTypes)
    {
        var names = this.inputPattern.GetGroupNames();
        foreach (var name in names)
        {
            var varType = variableTypes.TryGetValue(name, out string? value) ? Type.GetType(value, true) : typeof(string);
            this.typeMap.Add(name, varType!);
        }
    }

    [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Prefer read-only.")]
    private IReadOnlyDictionary<string, object> GetValues(Match match)
    {
        var result = new Dictionary<string, object>();
        for (int i = 0; i < match.Groups.Count; i++)
        {
            string name = this.inputPattern.GroupNameFromNumber(i);
            object value = Convert.ChangeType(match.Groups[i].Value, this.typeMap[name]);

            result.Add(name, value);
        }
        return result;
    }
}
