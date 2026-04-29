using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Extension methods for <see cref="IBrowserCaptureService"/> to simplify capturing resources from web pages using
/// navigation and timing options.
/// </summary>
/// <remarks>
/// <para>
/// These methods provide convenient overloads for capturing resources by constructing <see cref="NavigationOptions"/> and
/// <see cref="CaptureTimingOptions"/> from common parameters such as URLs, file extensions, and timeouts.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/> and may stop in-flight work.
/// </para>
/// </remarks>
public static class BrowserCaptureServiceExtensions
{
    /// <summary>
    /// Captures resources matching the specified file extensions for the given URL using the provided
    /// <see cref="IBrowserCaptureService"/>.
    /// </summary>
    /// <param name="service">The capture service to use.  Must not be <see langword="null"/>.</param>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="url">The target URL to navigate to.  Must not be <see langword="null"/>.</param>
    /// <param name="fileExtensions">Array of file extensions to capture (e.g., ".ts", ".m4s").  Must not be
    /// <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="refererUrl">Optional referer URL for navigation.</param>
    /// <param name="navigationTimeout">Optional timeout for page navigation.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing resources.</param>
    /// <param name="networkCallsTimeout">Optional timeout for network calls during capture.</param>
    /// <param name="pollInterval">Optional polling interval for completion checks.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses during navigation
    /// and resource capture.</param>
    /// <param name="shouldCompleteCapture">Optional predicate to determine when capture is complete.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="url"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    public static Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        this IBrowserCaptureService service,
        IBrowserSession session,
        Uri url,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        Uri? refererUrl = null,
        TimeSpan? navigationTimeout = null,
        TimeSpan? networkIdleTimeout = null,
        TimeSpan? networkCallsTimeout = null,
        TimeSpan? pollInterval = null,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(url);

        var navOptions = new NavigationOptions(url, refererUrl, navigationTimeout);
        var captureTimingOptions = new CaptureTimingOptions(networkIdleTimeout, networkCallsTimeout, pollInterval);

        return service.NavigateAndCaptureResourcesAsync(
            session, navOptions, fileExtensions, cancellationToken, rewriteSpec, shouldCompleteCapture, captureTimingOptions);
    }

    /// <summary>
    /// Captures resources matching the specified URLs for the given navigation using the provided
    /// <see cref="IBrowserCaptureService"/>.
    /// </summary>
    /// <param name="service">The capture service to use.  Must not be <see langword="null"/>.</param>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="url">The target URL to navigate to.  Must not be <see langword="null"/>.</param>
    /// <param name="urlsToCapture">Array of URLs to capture.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="refererUrl">Optional referer URL for navigation.</param>
    /// <param name="navigationTimeout">Optional timeout for page navigation.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing resources.</param>
    /// <param name="networkCallsTimeout">Optional timeout for network calls during capture.</param>
    /// <param name="pollInterval">Optional polling interval for completion checks.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses during navigation
    /// and resource capture.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> or <paramref name="url"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    public static Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
       this IBrowserCaptureService service,
       IBrowserSession session,
       Uri url,
       Uri[] urlsToCapture,
       CancellationToken cancellationToken,
       Uri? refererUrl = null,
       TimeSpan? navigationTimeout = null,
       TimeSpan? networkIdleTimeout = null,
       TimeSpan? networkCallsTimeout = null,
       TimeSpan? pollInterval = null,
       RewriteSpec? rewriteSpec = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(url);

        var navOptions = new NavigationOptions(url, refererUrl, navigationTimeout);
        var captureTimingOptions = new CaptureTimingOptions(networkIdleTimeout, networkCallsTimeout, pollInterval);

        return service.NavigateAndCaptureResourcesAsync(
            session, navOptions, urlsToCapture, cancellationToken, rewriteSpec, captureTimingOptions);
    }

    /// <summary>
    /// Captures resources matching the specified capture specification for the given URL using the provided
    /// <see cref="IBrowserCaptureService"/>.
    /// </summary>
    /// <param name="service">The capture service to use.  Must not be <see langword="null"/>.</param>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="url">The target URL to navigate to.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="refererUrl">Optional referer URL for navigation.</param>
    /// <param name="navigationTimeout">Optional timeout for page navigation.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing resources.</param>
    /// <param name="networkCallsTimeout">Optional timeout for network calls during capture.</param>
    /// <param name="pollInterval">Optional polling interval for completion checks.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/>, <paramref name="session"/>,
    /// <paramref name="url"/>, or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    public static Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        this IBrowserCaptureService service,
        IBrowserSession session,
        Uri url,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        Uri? refererUrl = null,
        TimeSpan? navigationTimeout = null,
        TimeSpan? networkIdleTimeout = null,
        TimeSpan? networkCallsTimeout = null,
        TimeSpan? pollInterval = null)
        => NavigateAndCaptureResourcesAsync(
            service,
            session,
            url,
            captureSpec,
            null,
            cancellationToken,
            refererUrl,
            navigationTimeout,
            networkIdleTimeout,
            networkCallsTimeout,
            pollInterval);

    /// <summary>
    /// Captures resources matching the specified capture specification for the given URL using the provided
    /// <see cref="IBrowserCaptureService"/>, with optional response rewriting.
    /// </summary>
    /// <param name="service">The capture service to use.  Must not be <see langword="null"/>.</param>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="url">The target URL to navigate to.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses during navigation
    /// and resource capture.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="refererUrl">Optional referer URL for navigation.</param>
    /// <param name="navigationTimeout">Optional timeout for page navigation.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing resources.</param>
    /// <param name="networkCallsTimeout">Optional timeout for network calls during capture.</param>
    /// <param name="pollInterval">Optional polling interval for completion checks.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/>, <paramref name="session"/>,
    /// <paramref name="url"/>, or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    public static Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        this IBrowserCaptureService service,
        IBrowserSession session,
        Uri url,
        CaptureSpec captureSpec,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken,
        Uri? refererUrl = null,
        TimeSpan? navigationTimeout = null,
        TimeSpan? networkIdleTimeout = null,
        TimeSpan? networkCallsTimeout = null,
        TimeSpan? pollInterval = null)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(url);
        ArgumentNullException.ThrowIfNull(captureSpec);

        var navOptions = new NavigationOptions(url, refererUrl, navigationTimeout);
        var captureTimingOptions = new CaptureTimingOptions(networkIdleTimeout, networkCallsTimeout, pollInterval);
        return service.NavigateAndCaptureResourcesAsync(
            session, navOptions, captureSpec, rewriteSpec, cancellationToken, captureTimingOptions);
    }
}
