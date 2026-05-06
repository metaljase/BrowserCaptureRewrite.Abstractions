namespace Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

/// <summary>
/// Provides helper methods for evaluating Multipurpose Internet Mail Extensions (MIME) types.
/// </summary>
internal static class ContentTypeHelper
{
    /// <summary>
    /// Normalises a set of content type strings by trimming whitespace, converting to lower-case, sorting any
    /// parameters alphabetically, and removing duplicates.
    /// </summary>
    /// <param name="contentTypes">
    /// The content type strings to normalise.  May be <see langword="null"/> or empty.  Each entry may include
    /// optional parameters after a semicolon (e.g., <c>; charset=utf-8</c>).
    /// </param>
    /// <returns>
    /// An array of normalised content type strings, each lower-case with parameters sorted alphabetically.  Returns
    /// an empty array if <paramref name="contentTypes"/> is <see langword="null"/> or contains only null/whitespace
    /// entries.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Null, empty, or whitespace-only entries are ignored.  The result contains only unique, valid media types.
    /// Parameters are sorted alphabetically so that order-insensitive comparison works correctly in
    /// <see cref="HasMatchingContentType"/>.
    /// </para>
    /// <example>
    /// <code>
    /// ContentTypeHelper.NormalizeContentTypes("APPLICATION/JSON; charset=utf-8", "VIDEO/MP4", "  text/html  ");
    /// // returns ["application/json; charset=utf-8", "video/mp4", "text/html"]
    /// </code>
    /// </example>
    /// </remarks>
    internal static string[] NormalizeContentTypes(params string[] contentTypes) =>
        contentTypes is null
            ? []
            : [.. contentTypes
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(NormalizeContentType)
                .Where(c => c.Length > 0)
                .Distinct()];

    /// <summary>
    /// Determines whether the specified response content type header value matches any of the given normalised
    /// content type candidates.
    /// </summary>
    /// <param name="responseContentType">
    /// The <c>Content-Type</c> response header value to check.  May be <see langword="null"/> or empty.
    /// </param>
    /// <param name="normalizedContentTypes">
    /// The list of normalised content type candidates to match against, as returned by
    /// <see cref="NormalizeContentTypes"/>.  Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the response content type matches any candidate; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Matching follows two rules depending on whether a candidate includes parameters:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <strong>No parameters</strong> — the candidate matches any response that shares the same media type,
    ///       regardless of what parameters the response includes.  For example, the candidate
    ///       <c>application/json</c> matches both <c>application/json</c> and
    ///       <c>application/json; charset=utf-8</c>.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <strong>With parameters</strong> — the media type must match and every parameter specified in the
    ///       candidate must be present in the response (subset match, order-insensitive, case-insensitive).  For
    ///       example, the candidate <c>application/json; charset=utf-8</c> does not match a bare response of
    ///       <c>application/json</c> (missing parameter), but does match
    ///       <c>application/json; charset=UTF-8; boundary=something</c>.
    ///     </description>
    ///   </item>
    /// </list>
    /// <para>
    /// Returns <see langword="false"/> if <paramref name="responseContentType"/> is <see langword="null"/>,
    /// empty, or whitespace, or if <paramref name="normalizedContentTypes"/> is empty.
    /// </para>
    /// </remarks>
    internal static bool HasMatchingContentType(string? responseContentType, IReadOnlyList<string> normalizedContentTypes)
    {
        if (string.IsNullOrWhiteSpace(responseContentType) || normalizedContentTypes.Count == 0)
            return false;

        var normalizedResponse = NormalizeContentType(responseContentType);
        if (normalizedResponse.Length == 0)
            return false;

        var responseParts = normalizedResponse.Split(';');
        var responseMediaType = responseParts[0].Trim();
        var responseParams = responseParts.Skip(1).Select(p => p.Trim()).Where(p => p.Length > 0).ToHashSet();

        foreach (var candidate in normalizedContentTypes)
        {
            var candidateParts = candidate.Split(';');
            var candidateMediaType = candidateParts[0].Trim();

            if (candidateMediaType != responseMediaType)
                continue;

            var candidateParams = candidateParts.Skip(1).Select(p => p.Trim()).Where(p => p.Length > 0).ToList();

            // No parameters on candidate: wildcard match — any response parameters are acceptable.
            // Parameterised candidate: every candidate parameter must be present in the response.
            if (candidateParams.Count == 0 || candidateParams.All(responseParams.Contains))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Normalises a single content type string by trimming whitespace, converting to lower-case, and sorting its
    /// parameters alphabetically.
    /// </summary>
    /// <param name="contentType">The content type string to normalise.</param>
    /// <returns>
    /// A normalised, lower-case content type string with parameters sorted alphabetically, or an empty string if
    /// the media-type portion is empty after trimming.
    /// </returns>
    private static string NormalizeContentType(string contentType)
    {
        var parts = contentType.ToLowerInvariant().Split(';');
        var mediaType = parts[0].Trim();
        if (mediaType.Length == 0)
            return string.Empty;

        var parameters = parts.Skip(1).Select(p => p.Trim()).Where(p => p.Length > 0).Order();
        var parameterSuffix = string.Join("; ", parameters);
        return parameterSuffix.Length > 0 ? $"{mediaType}; {parameterSuffix}" : mediaType;
    }

    /// <summary>
    /// Determines whether the specified content type is considered a text-based format.
    /// </summary>
    /// <param name="contentType">The content-type header value to evaluate.</param>
    /// <returns>
    /// <see langword="true"/> if the content type indicates text, JSON, XML, or script; otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool IsTextBasedContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        // Ignore parameters like '; charset=utf-8'.
        var mimeType = contentType.Split(';')[0].Trim().ToLowerInvariant();

        return mimeType.StartsWith("text/")
            || mimeType.EndsWith("+json")
            || mimeType.Contains("json")
            || mimeType.EndsWith("+xml")
            || (mimeType.Contains("xml")
                && !mimeType.Contains("officedocument")
                && !mimeType.Contains("openxmlformats"))
            || mimeType.Contains("javascript")
            || mimeType.Contains("x-www-form-urlencoded")
            || mimeType == "application/graphql";
    }
}
