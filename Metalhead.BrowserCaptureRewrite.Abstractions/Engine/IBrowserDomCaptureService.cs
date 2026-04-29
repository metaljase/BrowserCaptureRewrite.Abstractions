using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Defines methods for capturing HTML and resources from web pages using a browser session.
/// </summary>
/// <remarks>
/// <para>
/// Implementations provide combined capture of response HTML, rendered HTML, and resources for a given navigation.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  Operations may throw
/// <see cref="OperationCanceledException"/> if cancelled or if the browser session is closed.
/// </para>
/// </remarks>
public interface IBrowserDomCaptureService
{
    /// <summary>
    /// Captures response HTML, rendered HTML, and resources for the specified navigation.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured HTML and resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Captures response HTML, rendered HTML, and resources for the specified navigation, with optional response rewriting.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured HTML and resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null);
}
