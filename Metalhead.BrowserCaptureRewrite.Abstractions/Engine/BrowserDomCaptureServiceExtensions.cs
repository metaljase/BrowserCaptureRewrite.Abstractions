using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Extension methods for <see cref="IBrowserDomCaptureService"/> to simplify capturing HTML and resources from web pages using
/// navigation and timing options.
/// </summary>
/// <remarks>
/// <para>
/// These methods provide convenient overloads for capturing HTML and resources by constructing <see cref="NavigationOptions"/> and
/// <see cref="CaptureTimingOptions"/> from common parameters such as URLs and timeouts.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/> and may stop in-flight work.
/// </para>
/// </remarks>
public static class BrowserDomCaptureServiceExtensions
{
    /// <summary>
    /// Captures response HTML, rendered HTML, and resources for the specified navigation using the provided
    /// <see cref="IBrowserDomCaptureService"/>.
    /// </summary>
    /// <param name="service">The DOM capture service to use.  Must not be <see langword="null"/>.</param>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="url">The target URL to navigate to.  Must not be <see langword="null"/>.</param>
    /// <param name="refererUrl">Optional referer URL for navigation.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="navigationTimeout">Optional timeout for page navigation.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="networkCallsTimeout">Optional timeout for network calls during capture.</param>
    /// <param name="pollInterval">Optional polling interval for completion checks.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured HTML and resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/>, <paramref name="session"/>,
    /// <paramref name="url"/>, or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    public static async Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
        this IBrowserDomCaptureService service,
        IBrowserSession session,
        Uri url,
        Uri? refererUrl,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        TimeSpan? navigationTimeout = null,
        TimeSpan? networkIdleTimeout = null,
        TimeSpan? networkCallsTimeout = null,
        TimeSpan? pollInterval = null)
        => await NavigateAndCaptureHtmlAndResourcesResultAsync(
            service,
            session,
            url,
            refererUrl,
            captureSpec,
            null,
            cancellationToken,
            navigationTimeout,
            networkIdleTimeout,
            networkCallsTimeout,
            pollInterval).ConfigureAwait(false);

    /// <summary>
    /// Captures response HTML, rendered HTML, and resources for the specified navigation using the provided
    /// <see cref="IBrowserDomCaptureService"/>, with optional response rewriting.
    /// </summary>
    /// <param name="service">The DOM capture service to use.  Must not be <see langword="null"/>.</param>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="url">The target URL to navigate to.  Must not be <see langword="null"/>.</param>
    /// <param name="refererUrl">Optional referer URL for navigation.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses during navigation
    /// and resource capture.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="navigationTimeout">Optional timeout for page navigation.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="networkCallsTimeout">Optional timeout for network calls during capture.</param>
    /// <param name="pollInterval">Optional polling interval for completion checks.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured HTML and resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/>, <paramref name="session"/>,
    /// <paramref name="url"/>, or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    public static async Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
        this IBrowserDomCaptureService service,
        IBrowserSession session,
        Uri url,
        Uri? refererUrl,
        CaptureSpec captureSpec,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken,
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

        return await service.NavigateAndCaptureHtmlAndResourcesResultAsync(
            session, navOptions, captureSpec, rewriteSpec, cancellationToken, captureTimingOptions)
            .ConfigureAwait(false);
    }
}
