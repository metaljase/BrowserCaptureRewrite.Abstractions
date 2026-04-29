namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Represents a resource captured from a browser session, including its URL, content, status code, and response headers.
/// </summary>
/// <remarks>
/// <para>
/// This record is used to model HTTP resources (such as files, scripts, or media segments) captured during browser automation
/// or resource capture operations.
/// </para>
/// <para>
/// The resource may contain text content, binary content, or both, depending on the capture scenario and content type.
/// The <see cref="HasText"/> and <see cref="HasBinary"/> properties indicate which content is present.
/// </para>
/// <para>
/// <see cref="ContentType"/>, <see cref="StatusCode"/>, and <see cref="ResponseHeaders"/> may be <see langword="null"/>
/// if not available from the HTTP response.
/// </para>
/// </remarks>
/// <param name="Url">The absolute URL of the captured resource.  Must not be <see langword="null"/>.</param>
/// <param name="TextContent">The text content of the resource, or <see langword="null"/> if not applicable.</param>
/// <param name="BinaryContent">The binary content of the resource, or <see langword="null"/> if not applicable.</param>
/// <param name="ContentType">The HTTP content type of the resource, or <see langword="null"/> if not available.</param>
/// <param name="StatusCode">The HTTP status code of the response, or <see langword="null"/> if not available.</param>
/// <param name="ResponseHeaders">The HTTP response headers, or <see langword="null"/> if not available.</param>
public sealed record CapturedResource(
    Uri Url,
    string? TextContent,
    byte[]? BinaryContent,
    string? ContentType,
    int? StatusCode,
    IReadOnlyDictionary<string, string>? ResponseHeaders)
{
    /// <summary>
    /// Gets a value indicating whether the resource has non-<see langword="null"/> text content.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="TextContent"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasText => TextContent is not null;

    /// <summary>
    /// Gets a value indicating whether the resource has non-<see langword="null"/> binary content.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="BinaryContent"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasBinary => BinaryContent is not null;
}
