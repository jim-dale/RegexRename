namespace RegexRename.Support;

using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;

public class InputSource
{
    private readonly string folder;
    private readonly string searchPatterns;

    public InputSource(string folder, string searchPatterns)
    {
        ArgumentException.ThrowIfNullOrEmpty(folder, nameof(folder));
        ArgumentException.ThrowIfNullOrEmpty(searchPatterns, nameof(searchPatterns));

        this.folder = folder;
        this.searchPatterns = searchPatterns;
    }

    public IEnumerable<string> GetFiles()
    {
        var matcher = new Matcher();
        matcher.AddIncludePatterns(this.searchPatterns.Split(';'));

        var results = matcher.GetResultsInFullPath(this.folder);

        foreach (var result in results)
        {
            yield return result;
        }
    }
}
