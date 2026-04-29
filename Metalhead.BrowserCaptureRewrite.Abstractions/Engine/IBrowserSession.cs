using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Represents a browser session capable of navigating to web pages and capturing content and resources, with optional HTTP
/// response rewriting.
/// </summary>
/// <remarks>
/// <para>
/// Implementations manage browser and page lifecycle, including resource cleanup.  Navigation and capture operations are
/// cancellable and resilient
/// to transient browser closure.
/// </para>
/// <para>
/// Use <see cref="Dispose"/> or <see cref="DisposeAsync"/> to release browser resources when finished.
/// </para>
/// </remarks>
public interface IBrowserSession : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Navigates to the specified URL and captures selected content and resources.
    /// </summary>
    /// <param name="captureParts">
    /// Specifies which parts of the page to capture (e.g., response HTML, rendered HTML, resources).
    /// </param>
    /// <param name="navOptions">
    /// Navigation options, including the target URL and optional referer.  Must not be <see langword="null"/>.
    /// </param>
    /// <param name="captureSpec">Resource capture specification.  Required if <paramref name="captureParts"/> includes
    /// resources;
    /// otherwise, may be <see langword="null"/>.</param>
    /// <param name="captureTimingOptions">Timing and completion options for navigation and resource capture.  Must not be
    /// <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.  Cancels navigation and capture if triggered.</param>
    /// <returns>
    /// A <see cref="PageCaptureResult"/> containing the captured response HTML, rendered HTML, captured resources, and status
    /// information.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="navOptions"/>, <c>navOptions.Url</c>, or <paramref name="captureTimingOptions"/> is <see langword="null"/>,
    /// or if <paramref name="captureSpec"/> is <see langword="null"/> when resources are to be captured.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="captureParts"/> is <see cref="PageCaptureParts.None"/>.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown if the navigation response has a retryable or 404 HTTP status code.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is cancelled via <paramref name="cancellationToken"/> or if the browser/page is closed during
    /// navigation or capture.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When capturing resources, completion can be determined by network idle, a custom predicate, or a timeout, as specified in
    /// <paramref name="captureTimingOptions"/> and <paramref name="captureSpec"/>.
    /// </para>
    /// <para>
    /// The method is resilient to transient browser closure and cancellation, and logs debug information for capture progress
    /// and errors.
    /// </para>
    /// </remarks>
    Task<PageCaptureResult> NavigateAndCaptureResultAsync(
        PageCaptureParts captureParts,
        NavigationOptions navOptions,
        CaptureSpec? captureSpec,
        CaptureTimingOptions captureTimingOptions,
        CancellationToken cancellationToken);

    /// <summary>
    /// Navigates to the specified URL and captures selected content and resources, optionally rewriting HTTP responses.
    /// </summary>
    /// <param name="captureParts">
    /// Specifies which parts of the page to capture (e.g., response HTML, rendered HTML, resources).
    /// </param>
    /// <param name="navOptions">Navigation options, including the target URL and optional referer.  Must not be
    /// <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Required if <paramref name="captureParts"/> includes
    /// resources; otherwise, may be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">
    /// Optional specification for rewriting HTTP responses during navigation and resource capture.
    /// </param>
    /// <param name="captureTimingOptions">Timing and completion options for navigation and resource capture.  Must not be
    /// <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.  Cancels navigation and capture if triggered.</param>
    /// <returns>
    /// A <see cref="PageCaptureResult"/> containing the captured response HTML, rendered HTML, captured resources, and status
    /// information.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="navOptions"/>, <c>navOptions.Url</c>, or <paramref name="captureTimingOptions"/> is <see langword="null"/>,
    /// or if <paramref name="captureSpec"/> is <see langword="null"/> when resources are to be captured.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="captureParts"/> is <see cref="PageCaptureParts.None"/>.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown if the navigation response has a retryable or 404 HTTP status code.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is cancelled via <paramref name="cancellationToken"/> or if the browser/page is closed during
    /// navigation or capture.
    /// </exception>
    /// <remarks>
    /// <para>
    /// If <paramref name="rewriteSpec"/> is provided, HTTP responses matching the rewrite criteria will be intercepted
    /// and optionally rewritten.
    /// </para>
    /// <para>
    /// When capturing resources, completion can be determined by network idle, a custom predicate, or a timeout, as specified in
    /// <paramref name="captureTimingOptions"/> and <paramref name="captureSpec"/>.
    /// </para>
    /// <para>
    /// The method is resilient to transient browser closure and cancellation, and logs debug information for capture progress
    /// and errors.
    /// </para>
    /// </remarks>
    Task<PageCaptureResult> NavigateAndCaptureResultAsync(
        PageCaptureParts captureParts,
        NavigationOptions navOptions,
        CaptureSpec? captureSpec,
        RewriteSpec? rewriteSpec,
        CaptureTimingOptions captureTimingOptions,
        CancellationToken cancellationToken);
}
