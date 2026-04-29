namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Represents configuration options for launching and controlling a browser instance.
/// </summary>
/// <remarks>
/// <para>
/// This class is typically bound to configuration (e.g., appsettings) using the section name <c>BrowserOptions</c>,
/// defined by the <see cref="SectionName"/> constant.
/// </para>
/// <para>
/// All properties are optional and have sensible defaults for Chromium-based automation in headful mode.
/// </para>
/// </remarks>
public sealed class BrowserOptions
{
    /// <summary>
    /// The configuration section name for browser options.
    /// </summary>
    public const string SectionName = "BrowserOptions";

    /// <summary>
    /// Gets or sets the browser engine to use (e.g., Chromium, Firefox, WebKit).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is <see cref="BrowserEngine.Chromium"/>.
    /// </para>
    /// </remarks>
    public BrowserEngine Browser { get; set; } = BrowserEngine.Chromium;

    /// <summary>
    /// Gets or sets the path to the browser executable.  If <see langword="null"/>, the default browser installation is used.
    /// </summary>
    public string? ExecutablePath { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the browser should run in headless mode.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is <see langword="false"/> (headful mode).
    /// </para>
    /// </remarks>
    public bool Headless { get; set; } = false;

    /// <summary>
    /// Gets or sets the user agent string to use for browser requests.  If <see langword="null"/>, the default user agent is used.
    /// </summary>
    public string? UserAgent { get; set; } = null;

    /// <summary>
    /// Gets or sets the viewport width in pixels.  If <see langword="null"/>, the default viewport width is used.
    /// </summary>
    public int? ViewportWidth { get; set; } = null;

    /// <summary>
    /// Gets or sets the viewport height in pixels.  If <see langword="null"/>, the default viewport height is used.
    /// </summary>
    public int? ViewportHeight { get; set; } = null;
}
