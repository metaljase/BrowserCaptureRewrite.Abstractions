namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Specifies navigation options for browser automation, including the target URL, optional referer, and page load timeout.
/// </summary>
/// <remarks>
/// <para>
/// Used to control navigation behaviour for resource capture and browser session operations.
/// </para>
/// <para>
/// <see cref="Url"/> must be a non-<see langword="null"/> absolute URI.  <see cref="RefererUrl"/> and
/// <see cref="PageLoadTimeout"/> are optional and may be <see langword="null"/>.
/// </para>
/// <para>
/// If <see cref="PageLoadTimeout"/> is <see langword="null"/>, the default timeout for the browser session is used.
/// </para>
/// </remarks>
/// <param name="Url">
/// The absolute URL to navigate to.  Must not be <see langword="null"/>.
/// </param>
/// <param name="RefererUrl">
/// The referer URL to send with the navigation request, or <see langword="null"/> to omit the referer.
/// </param>
/// <param name="PageLoadTimeout">
/// The maximum duration to wait for the page to load, or <see langword="null"/> to use the default timeout.
/// </param>
public sealed record NavigationOptions(Uri Url, Uri? RefererUrl = null, TimeSpan? PageLoadTimeout = null);
