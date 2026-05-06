using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Defines methods for navigating to web pages and capturing resources using a browser session.
/// </summary>
/// <remarks>
/// <para>
/// Implementations provide high-level resource capture operations, supporting file extension, URL, or custom capture specifications.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  Operations may throw
/// <see cref="OperationCanceledException"/> if cancelled or if the browser session is closed.
/// </para>
/// </remarks>
public interface IBrowserCaptureService
{
    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given file extensions.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="fileExtensions">
    /// Array of file extensions to capture (e.g., ".ts", ".m4s").  Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="shouldCompleteCapture">Optional predicate to determine when capture is complete.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="fileExtensions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="fileExtensions"/> is empty or contains no valid extensions.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    [Obsolete("Use NavigateAndCaptureResourcesByFileExtensionAsync instead.  This method will be removed in a future major version.")]
    Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources whose URLs match the given file extensions.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="fileExtensions">
    /// Array of file extensions to capture (e.g., ".ts", ".m4s").  Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="shouldCompleteCapture">Optional predicate to determine when capture is complete.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="fileExtensions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="fileExtensions"/> is empty or contains no valid extensions.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesByFileExtensionAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given file extensions, returning the full capture result.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="fileExtensions">Array of file extensions to capture.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="shouldCompleteCapture">Optional predicate to determine when capture is complete.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured resources and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="fileExtensions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="fileExtensions"/> is empty or contains no valid extensions.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    [Obsolete("Use NavigateAndCaptureResourcesByFileExtensionResultAsync instead.  This method will be removed in a future major version.")]
    Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources whose URLs match the given file extensions, returning the full
    /// capture result.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="fileExtensions">Array of file extensions to capture.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="shouldCompleteCapture">Optional predicate to determine when capture is complete.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured resources and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="fileExtensions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="fileExtensions"/> is empty or contains no valid extensions.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResourcesByFileExtensionResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given URLs.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="urlsToCapture">Array of URLs to capture.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="urlsToCapture"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="urlsToCapture"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    [Obsolete("Use NavigateAndCaptureResourcesByUrlAsync instead.  This method will be removed in a future major version.")]
    Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given URLs.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="urlsToCapture">Array of URLs to capture.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="urlsToCapture"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="urlsToCapture"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesByUrlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given URLs, returning the full capture result.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="urlsToCapture">Array of URLs to capture.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured resources and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="urlsToCapture"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="urlsToCapture"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    [Obsolete("Use NavigateAndCaptureResourcesByUrlResultAsync instead.  This method will be removed in a future major version.")]
    Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given URLs, returning the full capture result.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="urlsToCapture">Array of URLs to capture.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured resources and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="urlsToCapture"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="urlsToCapture"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResourcesByUrlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources whose HTTP response <c>Content-Type</c> header matches any
    /// of the given media types.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="contentTypes">
    /// Array of media types to capture (e.g., <c>"application/json"</c>, <c>"video/mp4"</c>).  Must not be
    /// <see langword="null"/> or empty.  An entry without parameters (e.g., <c>"application/json"</c>) matches any
    /// response with that media type regardless of its parameters.  An entry with parameters (e.g.,
    /// <c>"application/json; charset=utf-8"</c>) requires every specified parameter to be present in the response
    /// (subset match, order-insensitive, case-insensitive).
    /// </param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="shouldCompleteCapture">Optional predicate to determine when capture is complete.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="contentTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="contentTypes"/> is empty or contains no valid media types.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesByContentTypeAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] contentTypes,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources whose HTTP response <c>Content-Type</c> header matches any
    /// of the given media types, returning the full capture result.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="contentTypes">
    /// Array of media types to capture (e.g., <c>"application/json"</c>, <c>"video/mp4"</c>).  Must not be
    /// <see langword="null"/> or empty.  An entry without parameters (e.g., <c>"application/json"</c>) matches any
    /// response with that media type regardless of its parameters.  An entry with parameters (e.g.,
    /// <c>"application/json; charset=utf-8"</c>) requires every specified parameter to be present in the response
    /// (subset match, order-insensitive, case-insensitive).
    /// </param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="shouldCompleteCapture">Optional predicate to determine when capture is complete.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured resources and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="contentTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="contentTypes"/> is empty or contains no valid media types.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResourcesByContentTypeResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] contentTypes,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given capture specification.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given capture specification, with optional response rewriting.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A read-only list of captured resources.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given capture specification, returning the full capture result.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured resources and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null);

    /// <summary>
    /// Navigates to the specified URL and captures resources matching the given capture specification, with optional response
    /// rewriting, returning the full capture result.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="captureSpec">Resource capture specification.  Must not be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <param name="captureTimingOptions">Optional timing and completion options.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing captured resources and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/>, <paramref name="navOptions"/>,
    /// or <paramref name="captureSpec"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null);
}
