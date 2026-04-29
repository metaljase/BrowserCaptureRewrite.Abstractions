using System.Text.RegularExpressions;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

/// <summary>
/// Provides helper methods for formatting durations and humanising enumeration values for display.
/// </summary>
/// <remarks>
/// <para>
/// All methods are <see langword="static"/> and intended for use in logging, diagnostics, and user-facing output where
/// human-readable formatting is required.
/// </para>
/// <para>
/// Duration formatting uses context-sensitive units and pluralisation.  Enum humanisation inserts spaces between words in
/// PascalCase or ALLCAPS names.
/// </para>
/// </remarks>
public static partial class HumanizeHelper
{
    /// <summary>
    /// Formats a <see cref="TimeSpan"/> value as a human-readable string using context-appropriate units and pluralisation.
    /// </summary>
    /// <param name="value">
    /// The duration to format.  Must be non-negative.
    /// </param>
    /// <returns>
    /// A string representing the duration in milliseconds, seconds, minutes, hours, or days, with appropriate precision and pluralisation.
    /// </returns>
    /// <remarks>
    /// <para>
    /// For durations less than one second, the value is shown in milliseconds.  For longer durations, the value is shown in
    /// seconds, minutes, hours, or days, with precision and unit chosen based on the magnitude.
    /// </para>
    /// <example>
    /// <code>
    /// HumanizeHelper.FormatDuration(TimeSpan.FromMilliseconds(500)); // "500 ms"
    /// HumanizeHelper.FormatDuration(TimeSpan.FromSeconds(5)); // "5 secs"
    /// HumanizeHelper.FormatDuration(TimeSpan.FromMinutes(2.5)); // "2.5 mins"
    /// HumanizeHelper.FormatDuration(TimeSpan.FromHours(1)); // "1 hr"
    /// </code>
    /// </example>
    /// </remarks>
    public static string FormatDuration(TimeSpan value)
    {
        static string Pluralize(double amount, string singular, string plural)
            => Math.Abs(amount - 1) < 0.0001 ? singular : plural;

        return value.TotalSeconds switch
        {
            < 1 => $"{value.TotalMilliseconds:0} ms",                                                              // < 1 second
            < 10 => $"{value.TotalSeconds:0.###} {Pluralize(value.TotalSeconds, "sec", "secs")}",   // < 10 seconds
            < 60 => $"{value.TotalSeconds:0.#} {Pluralize(value.TotalSeconds, "sec", "secs")}",     // < 1 minute
            < 600 => $"{value.TotalMinutes:0.##} {Pluralize(value.TotalMinutes, "min", "mins")}",   // < 10 minutes
            < 3600 => $"{value.TotalMinutes:0.#} {Pluralize(value.TotalMinutes, "min", "mins")}",   // < 1 hour
            < 36000 => $"{value.TotalHours:0.##} {Pluralize(value.TotalHours, "hr", "hrs")}",       // < 10 hours
            < 86400 => $"{value.TotalHours:0.#} {Pluralize(value.TotalHours, "hr", "hrs")}",        // < 1 day
            < 864000 => $"{value.TotalDays:0.##} {Pluralize(value.TotalDays, "day", "days")}",      // < 10 days
            _ => $"{value.TotalDays:0.#} {Pluralize(value.TotalDays, "day", "days")}"               // >= 10 days
        };
    }

    /// <summary>
    /// Converts an <see cref="Enum"/> value to a human-readable string by inserting spaces between words in PascalCase or ALLCAPS names.
    /// </summary>
    /// <param name="value">
    /// The enumeration value to humanise.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A string with spaces inserted between words in the enum name, or the value's string representation if the name cannot be determined.
    /// </returns>
    /// <remarks>
    /// <para>
    /// For example, an enum value named <c>PageLoadStatus</c> will be returned as "Page Load Status".
    /// </para>
    /// </remarks>
    public static string HumanizeEnum(Enum value)
    {
        var name = Enum.GetName(value.GetType(), value);
        return name is null ? value.ToString() : HumanizeEnumRegex().Replace(name, " ");
    }

    /// <summary>
    /// Gets the regular expression used to insert spaces between words in PascalCase or ALLCAPS enum names.
    /// </summary>
    /// <returns>
    /// A <see cref="Regex"/> instance for splitting enum names into words.
    /// </returns>
    /// <remarks>
    /// This method is <see langword="partial"/> and generated at compile time.
    /// </remarks>
    [GeneratedRegex("(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])")]
    private static partial Regex HumanizeEnumRegex();
}
