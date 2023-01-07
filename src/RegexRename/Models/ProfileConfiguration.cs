namespace RegexRename.Models;

using System;
using System.Collections.Generic;

public partial class ProfileConfiguration
{
    public string? DefaultProfile { get; set; }
    public IDictionary<string, Profile> Items { get; } = new Dictionary<string, Profile>(StringComparer.InvariantCultureIgnoreCase);
}
