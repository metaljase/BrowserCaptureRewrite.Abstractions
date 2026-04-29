using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Services;

/// <summary>
/// Defines a service for creating browser sessions, supporting both standard and sign-in flows.
/// </summary>
/// <remarks>
/// <para>
/// Implementations must provide asynchronous session creation, optionally supporting sign-in navigation and completion detection.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancellation is requested
/// before or during session creation, an <see cref="OperationCanceledException"/> is thrown.  If the browser or page is externally
/// closed, an <see cref="OperationCanceledException"/> is also thrown.
/// </para>
/// </remarks>
public interface IBrowserSessionService
{
    /// <summary>
    /// Creates a new standard browser session without a sign-in flow.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.  The task result is an <see cref="IBrowserSession"/>
    /// representing the created session.
    /// </returns>
    /// <exception cref="BrowserSessionInitializationException">Thrown for general session creation failures.</exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is cancelled before or during session creation.
    /// </exception>
    Task<IBrowserSession> CreateBrowserSessionOrThrowAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new browser session, optionally performing a sign-in flow if a sign-in URL is provided.
    /// </summary>
    /// <param name="signInUrl">The URL to use for sign-in.  If <see langword="null"/>, a standard session is created.</param>
    /// <param name="signedInUrl">The URL to assume sign-in is complete when navigated to.  May be <see langword="null"/>.</param>
    /// <param name="signInOptions">
    /// The sign-in timing options controlling how long to wait before assuming sign-in is complete and the maximum page
    /// load timeout.  Must not be <see langword="null"/>.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.  The task result is an <see cref="IBrowserSession"/> representing
    /// the created session.
    /// </returns>
    /// <exception cref="SignInException">Thrown if sign-in fails or is cancelled.</exception>
    /// <exception cref="BrowserSessionInitializationException">
    /// Thrown for general session creation failures, including when the browser automation engine is not available
    /// (<see cref="BrowserSessionInitializationFailureReason.EngineNotAvailable"/>).
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is cancelled before or during session creation.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When <paramref name="signInUrl"/> is provided, sign-in completion is detected by navigation to
    /// <paramref name="signedInUrl"/> if supplied, or by waiting for the duration specified by
    /// <paramref name="assumeSignedInAfter"/> if <paramref name="signedInUrl"/> is <see langword="null"/>.
    /// </para>
    /// </remarks>
    Task<IBrowserSession> CreateBrowserSessionOrThrowAsync(
        Uri? signInUrl, Uri? signedInUrl, SignInOptions signInOptions, CancellationToken cancellationToken);
}
