using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Factories;

/// <summary>
/// Defines a contract for creating browser session instances with specified options.
/// </summary>
/// <remarks>
/// <para>
/// Implementations are responsible for initialising and configuring browser sessions, including engine selection,
/// sign-in behaviour, and resilience.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/>.
/// </para>
/// </remarks>
public interface IBrowserSessionFactory
{
    /// <summary>
    /// Creates a new browser session using the specified options.
    /// </summary>
    /// <param name="options">The session options to use for browser configuration.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.  The task result contains the created <see cref="IBrowserSession"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is cancelled.</exception>
    Task<IBrowserSession> CreateSessionAsync(SessionOptions options, CancellationToken cancellationToken);
}
