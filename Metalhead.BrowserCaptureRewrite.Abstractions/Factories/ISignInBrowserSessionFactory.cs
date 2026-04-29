using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Factories;

/// <summary>
/// Defines a contract for creating browser session instances that are initialised for sign-in workflows.
/// </summary>
/// <remarks>
/// <para>
/// Implementations are responsible for initialising and configuring browser sessions that begin at a sign-in URL, supporting
/// engine selection, sign-in detection, and resilience.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/>.
/// </para>
/// </remarks>
public interface ISignInBrowserSessionFactory
{
    /// <summary>
    /// Creates a new browser session initialised for sign-in using the specified sign-in URL and session options.
    /// </summary>
    /// <param name="signInUrl">The URL to use for the sign-in navigation.  Must not be <see langword="null"/>.</param>
    /// <param name="options">The session options to use for browser configuration.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.  The task result contains the created <see cref="IBrowserSession"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="signInUrl"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<IBrowserSession> CreateSignInSessionAsync(Uri signInUrl, SessionOptions options, CancellationToken cancellationToken);
}
