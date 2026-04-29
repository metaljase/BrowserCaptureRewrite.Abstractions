namespace Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

/// <summary>
/// Specifies which parts of a web page should be captured during a navigation and capture operation.
/// </summary>
/// <remarks>
/// <para>
/// This enumeration supports bitwise combination of its values to indicate multiple capture parts.
/// </para>
/// <para>
/// Typical usage includes capturing the original HTTP response HTML, the rendered HTML after JavaScript execution, and/or additional resources
/// such as images, scripts, or media segments.
/// </para>
/// </remarks>
[Flags]
public enum PageCaptureParts
{
    /// <summary>
    /// No parts are captured.
    /// </summary>
    None = 0,

    /// <summary>
    /// Capture the original HTTP response HTML.
    /// </summary>
    ResponseHtml = 1 << 0,

    /// <summary>
    /// Capture the rendered HTML after JavaScript execution.
    /// </summary>
    RenderedHtml = 1 << 1,

    /// <summary>
    /// Capture additional resources (e.g., images, scripts, media segments).
    /// </summary>
    Resources = 1 << 2,

    /// <summary>
    /// Capture all available parts (response HTML, rendered HTML, and resources).
    /// </summary>
    All = ResponseHtml | RenderedHtml | Resources
}
