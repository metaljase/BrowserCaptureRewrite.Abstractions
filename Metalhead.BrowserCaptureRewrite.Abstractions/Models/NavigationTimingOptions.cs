namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Holds timing options that control the maximum time allowed for a page to load during navigation.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="PageLoadTimeoutSeconds"/> is stored as a nullable <see langword="float"/> value (in seconds) and exposed
/// as a nullable <see cref="TimeSpan"/> via <see cref="PageLoadTimeout"/>.  A <see langword="null"/> value indicates
/// that the default of <see cref="DefaultPageLoadTimeout"/> (30 seconds) should be used.
/// </para>
/// <para>
/// This class can be bound directly from configuration using the section name defined by <see cref="SectionName"/>.
/// </para>
/// </remarks>
public sealed class NavigationTimingOptions
{
    /// <summary>
    /// The configuration section name for binding <see cref="NavigationTimingOptions"/> from settings.
    /// </summary>
    public const string SectionName = "NavigationTimingOptions";

    /// <summary>
    /// Initialises a new instance with <see cref="PageLoadTimeoutSeconds"/> set to <see langword="null"/>.
    /// </summary>
    public NavigationTimingOptions()
    {
        PageLoadTimeoutSeconds = null;
    }

    /// <summary>
    /// Initialises a new instance with the page load timeout specified in seconds.
    /// </summary>
    /// <param name="pageLoadTimeoutSeconds">
    /// The maximum number of seconds to wait for a page to load, or <see langword="null"/> to use
    /// <see cref="DefaultPageLoadTimeout"/>.  Must be greater than 0 if specified.
    /// </param>
    public NavigationTimingOptions(float? pageLoadTimeoutSeconds = null)
    {
        PageLoadTimeoutSeconds = pageLoadTimeoutSeconds;
    }

    /// <summary>
    /// Initialises a new instance with the page load timeout specified as a <see cref="TimeSpan"/>, truncated to whole
    /// seconds.
    /// </summary>
    /// <param name="pageLoadTimeout">
    /// The maximum duration to wait for a page to load, or <see langword="null"/> to use
    /// <see cref="DefaultPageLoadTimeout"/>.  Must represent a positive duration if specified.
    /// </param>
    public NavigationTimingOptions(TimeSpan? pageLoadTimeout)
    {
        PageLoadTimeoutSeconds = pageLoadTimeout.HasValue ? (float)pageLoadTimeout.Value.TotalSeconds : null;
    }

    /// <summary>
    /// The default value returned by <see cref="PageLoadTimeout"/> when <see cref="PageLoadTimeoutSeconds"/> is
    /// <see langword="null"/>.
    /// </summary>
    public static readonly TimeSpan DefaultPageLoadTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum number of seconds to wait for a page to load.  Must be greater than 0 if not
    /// <see langword="null"/>.
    /// </summary>
    public float? PageLoadTimeoutSeconds { get; set; }

    /// <summary>
    /// Returns the page load timeout as a <see cref="TimeSpan"/>, or <see cref="DefaultPageLoadTimeout"/> when
    /// <see cref="PageLoadTimeoutSeconds"/> is <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing <see cref="PageLoadTimeoutSeconds"/>, or
    /// <see cref="DefaultPageLoadTimeout"/> if <see cref="PageLoadTimeoutSeconds"/> is <see langword="null"/>.
    /// </returns>
    public TimeSpan PageLoadTimeout() => PageLoadTimeoutSeconds.HasValue
        ? TimeSpan.FromSeconds((double)PageLoadTimeoutSeconds.Value)
        : DefaultPageLoadTimeout;
}