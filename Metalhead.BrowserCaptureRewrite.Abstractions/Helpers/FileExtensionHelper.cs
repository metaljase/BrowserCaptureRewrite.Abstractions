namespace Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

/// <summary>
/// Provides helper methods for normalising and matching file extensions.
/// </summary>
/// <remarks>
/// <para>
/// All methods are <see langword="static"/> and intended for internal use across projects that require consistent
/// file extension handling.
/// </para>
/// <para>
/// File extensions are normalised to lower-case, prefixed with a dot, and deduplicated.
/// </para>
/// </remarks>
internal static class FileExtensionHelper
{
    /// <summary>
    /// Normalises a set of file extensions by trimming whitespace, ensuring a leading dot, converting to lower-case,
    /// and removing duplicates.
    /// </summary>
    /// <param name="extensions">
    /// The file extensions to normalise.  May be <see langword="null"/> or empty.  Each extension may or may not
    /// start with a dot.
    /// </param>
    /// <returns>
    /// An array of normalised file extensions, each lower-case and starting with a dot.  Returns an empty array if
    /// <paramref name="extensions"/> is <see langword="null"/> or contains only null/whitespace entries.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Null, empty, or whitespace-only entries are ignored.  The result contains only unique, valid extensions.
    /// </para>
    /// <example>
    /// <code>
    /// FileExtensionHelper.NormalizeFileExtensions("MP4", ".jpg", "  png  ", null, ""); // returns [".mp4", ".jpg", ".png"]
    /// </code>
    /// </example>
    /// </remarks>
    internal static string[] NormalizeFileExtensions(params string[] extensions) =>
        extensions is null
            ? []
            : [.. extensions
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())
                .Select(e => e.StartsWith('.') ? e : "." + e)
                .Select(e => e.ToLowerInvariant())
                .Distinct()];

    /// <summary>
    /// Determines whether the specified file path ends with any of the given normalised file extensions.
    /// </summary>
    /// <param name="path">
    /// The file path to check.  May be <see langword="null"/> or empty.
    /// </param>
    /// <param name="normalizedExtensions">
    /// The list of normalised file extensions to match against.  Must not be <see langword="null"/>.  Each extension
    /// should be lower-case and start with a dot.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="path"/> ends with any of the specified extensions (case-insensitive); otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Returns <see langword="false"/> if <paramref name="path"/> is <see langword="null"/> or empty, or if
    /// <paramref name="normalizedExtensions"/> is empty.
    /// </para>
    /// </remarks>
    internal static bool HasMatchingFileExtension(string path, IReadOnlyList<string> normalizedExtensions)
    {
        if (string.IsNullOrEmpty(path) || normalizedExtensions.Count == 0)
            return false;

        foreach (var ext in normalizedExtensions)
            if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }
}