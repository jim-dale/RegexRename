namespace RegexRename.Models;

using System.Collections.Generic;

public class Profile
{
    public string? Folder { get; set; }
    public string? FileSearchPattern { get; set; }
    public string? InputPattern { get; set; }
    public string? OutputPattern { get; set; }
    public IDictionary<string, string>? Variables { get; set; }

    public string ToLongString(string name)
    {
        name = StringifyProperty(name);
        string folder = StringifyProperty(this.Folder);
        string fileSearchPattern = StringifyProperty(this.FileSearchPattern);
        string inputPattern = StringifyProperty(this.InputPattern);
        string outputPattern = StringifyProperty(this.OutputPattern);

        return $"{name}, {folder}, {fileSearchPattern}, {inputPattern}, {outputPattern}";
    }

    public static string ToShortString(string name)
    {
        return StringifyProperty(name);
    }

    private static string StringifyProperty(string? value)
    {
        return value == null ? "(Not Set)" : "'" + value + "'";
    }
}
