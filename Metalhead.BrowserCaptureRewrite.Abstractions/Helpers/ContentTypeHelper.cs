namespace Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

/// <summary>
/// Provides helper methods for evaluating Multipurpose Internet Mail Extensions (MIME) types.
/// </summary>
internal static class ContentTypeHelper
{
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
