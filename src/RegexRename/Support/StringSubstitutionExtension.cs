namespace Mindplay.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

// https://gist.github.com/mindplay-dk/896724#file-stringsubstitutionextension-cs
// Changed dictionary parameter from IDictionary<String, Object> to IReadOnlyDictionary<String, Object>

/// <summary>
/// This extension provides an alternative to <see cref="string.Format"/> allowing the
/// use of an <see cref="IDictionary{String,Object}"/> to replace named (rather than
/// numbered) tokens in a string template.
/// 
/// Usage is Similar to that of String.Format(), but the string templates use names
/// instead of numbers when referencing the values to substitute - and the input is
/// a dictionary rather than an array.
/// 
/// The following unit test provides a demonstration of how to use this extension:
/// 
/// <code>
/// var replacements = new Dictionary<String, object>()
///                        {
///                            { "date1", new DateTime(2009, 7, 1) },
///                            { "hiTime", new TimeSpan(14, 17, 32) },
///                            { "hiTemp", 62.1m },
///                            { "loTime", new TimeSpan(3, 16, 10) },
///                            { "loTemp", 54.8m }
///                        };
/// 
/// var template =
///     "Temperature on {date1:d}:\n{hiTime,11}: {hiTemp} degrees (hi)\n{loTime,11}: {loTemp} degrees (lo)";
/// 
/// var expected = "Temperature on 7/1/2009:\n   14:17:32: 62.1 degrees (hi)\n   03:16:10: 54.8 degrees (lo)";
/// 
/// var result = template.Subtitute(replacements);
/// 
/// Assert.IsTrue(
///     result == expected,
///     "string template mismatch:\n" + result + "\nexpected:\n" + expected);
/// </code>
/// 
/// You may contrast this example with the reference example provided for String.Format():
/// 
/// http://msdn.microsoft.com/en-us/library/1ksz8yb7.aspx
/// </summary>
public static partial class StringSubstitutionExtension
{
    private static readonly Regex Pattern = ParametersRegex();

    /// <summary>
    /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified dictionary.
    /// </summary>
    public static string Subtitute(this string template, IReadOnlyDictionary<string, object> dictionary)
    {
        return template.Subtitute(CultureInfo.InvariantCulture, dictionary);
    }

    /// <summary>
    /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified dictionary.
    /// A specified parameter supplies culture-specific formatting information.
    /// </summary>
    [SuppressMessage("Performance", "CA1854:Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method", Justification = "External code.")]
    [SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "External code.")]
    public static string Subtitute(this string template, IFormatProvider formatProvider, IReadOnlyDictionary<string, object> dictionary)
    {
        var map = new Dictionary<string, int>();

        var list = new List<object?>();

        var format = Pattern.Replace(
            template,
            match =>
            {
                var name = match.Groups[1].Captures[0].Value;

                if (!map.ContainsKey(name))
                {
                    map[name] = map.Count;
                    list.Add(dictionary.ContainsKey(name) ? dictionary[name] : null);
                }

                return "{" + map[name] + match.Groups[2].Captures[0].Value + "}";
            }
            );

        return formatProvider == null
            ? string.Format(format, list.ToArray())
            : string.Format(formatProvider, format, list.ToArray());
    }

    [GeneratedRegex("(?<!\\{)\\{(\\w+)([^\\}]*)\\}")]
    private static partial Regex ParametersRegex();
}
