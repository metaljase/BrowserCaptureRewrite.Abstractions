using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Defines methods for capturing response and rendered HTML from web pages using a browser session.
/// </summary>
/// <remarks>
/// <para>
/// Implementations provide access to raw and rendered HTML, with optional resource and status metadata.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  Operations may throw
/// <see cref="OperationCanceledException"/> if cancelled or if the browser session is closed.
/// </para>
/// </remarks>
public interface IBrowserDomService
{
    /// <summary>
    /// Gets only the response (raw navigation response) HTML without waiting for network idle.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The raw response HTML, or <see langword="null"/> if not available.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<string?> NavigateAndCaptureResponseHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only the response (raw navigation response) HTML without waiting for network idle, with optional response rewriting.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The raw response HTML, or <see langword="null"/> if not available.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<string?> NavigateAndCaptureResponseHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only the response (raw navigation response) HTML without waiting for network idle.
    /// Returns the raw capture result, which may include capture metadata/status fields.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing the response HTML and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResponseHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only the response (raw navigation response) HTML without waiting for network idle, with optional response rewriting.
    /// Returns the raw capture result, which may include capture metadata/status fields.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing the response HTML and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResponseHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only the rendered (post-JS) HTML for the specified navigation.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The rendered HTML, or <see langword="null"/> if not available.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<string?> NavigateAndCaptureRenderedHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only the rendered (post-JS) HTML for the specified navigation, with optional response rewriting.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The rendered HTML, or <see langword="null"/> if not available.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<string?> NavigateAndCaptureRenderedHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only the rendered (post-JS) HTML for the specified navigation.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// Returns the raw capture result, which may include capture metadata/status fields.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing the rendered HTML and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureRenderedHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets only the rendered (post-JS) HTML for the specified navigation, with optional response rewriting.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// Returns the raw capture result, which may include capture metadata/status fields.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing the rendered HTML and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureRenderedHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the response and rendered (post-JS) HTML.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A tuple containing the response HTML and rendered HTML.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<(string? ResponseHtml, string? RenderedHtml)> NavigateAndCaptureResponseAndRenderedHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the response and rendered (post-JS) HTML, with optional response rewriting.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A tuple containing the response HTML and rendered HTML.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<(string? ResponseHtml, string? RenderedHtml)> NavigateAndCaptureResponseAndRenderedHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the response and rendered (post-JS) HTML.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// Returns the raw capture result, which may include capture metadata/status fields.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing the response and rendered HTML, and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResponseAndRenderedHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the response and rendered (post-JS) HTML, with optional response rewriting.
    /// Optionally waits for NetworkIdle using the supplied timeout for the rendered DOM.
    /// Returns the raw capture result, which may include capture metadata/status fields.
    /// </summary>
    /// <param name="session">The browser session to use.  Must not be <see langword="null"/>.</param>
    /// <param name="navOptions">Navigation options, including the target URL.  Must not be <see langword="null"/>.</param>
    /// <param name="networkIdleTimeout">Optional timeout to wait for network idle before capturing rendered HTML.</param>
    /// <param name="rewriteSpec">Optional specification for rewriting HTTP responses.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="PageCaptureResult"/> containing the response and rendered HTML, and status information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="navOptions"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<PageCaptureResult> NavigateAndCaptureResponseAndRenderedHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default);
}
