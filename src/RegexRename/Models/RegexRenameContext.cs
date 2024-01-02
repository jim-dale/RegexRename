namespace RegexRename.Models;

public class RegexRenameContext
{
    public bool ShowHelp { get; set; }
    public bool ShowExampleSettings { get; set; }
    public bool ListLongProfiles { get; set; }
    public bool ListShortProfiles { get; set; }
    public bool WhatIf { get; set; }

    public Profile Profile { get; set; } = new();

    public int ExitCode { get; set; }
    public IList<string> ErrorList { get; } = new List<string>();
}
