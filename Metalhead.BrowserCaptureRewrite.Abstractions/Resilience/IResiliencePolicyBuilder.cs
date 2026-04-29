using Polly.Fallback;
using Polly.Retry;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

/// <summary>
/// Defines a contract for building resilience policies for browser automation and HTTP operations.
/// </summary>
/// <remarks>
/// Implementations must provide policies for transport retries, timeout retries, and fallback behaviour.
/// <para>
/// All policy-building operations must support cancellation via <see cref="CancellationToken"/>.  If cancellation is requested,
/// in-flight work must be stopped and an <see cref="OperationCanceledException"/> thrown.
/// </para>
/// </remarks>
public interface IResiliencePolicyBuilder
{
    /// <summary>
    /// Builds a fallback policy for the specified URL and exception predicate.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation being protected.</typeparam>
    /// <param name="url">The target URL for the operation.  Must not be <see langword="null"/>.</param>
    /// <param name="predicate">A predicate that determines which exceptions are handled by the fallback policy.  Must not be
    /// <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="AsyncFallbackPolicy{TResult}"/> that handles exceptions as specified by <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="url"/> or <paramref name="predicate"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if <paramref name="cancellationToken"/> is cancelled before or during policy construction.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The fallback policy will handle HTTP, timeout, and predicate-matching exceptions, and will propagate connectivity-related
    /// exceptions as appropriate.
    /// </para>
    /// </remarks>
    AsyncFallbackPolicy<TResult> BuildFallbackPolicy<TResult>(Uri url, Func<Exception, bool> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Builds a timeout retry policy for the specified URL, retry delays, and timeout.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation being protected.</typeparam>
    /// <param name="url">The target URL for the operation.  Must not be <see langword="null"/>.</param>
    /// <param name="delays">The sequence of retry delays to use.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="timeout">The timeout duration for the operation, or <see langword="null"/> to use the default.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="AsyncRetryPolicy{TResult}"/> that retries on timeout and retryable cancellation exceptions.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="url"/> or <paramref name="delays"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if <paramref name="cancellationToken"/> is cancelled before or during policy construction.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The retry policy will handle <see cref="TimeoutException"/> and retryable <see cref="TaskCanceledException"/> instances.
    /// </para>
    /// </remarks>
    AsyncRetryPolicy<TResult> BuildTimeoutRetryPolicy<TResult>(
        Uri url, IReadOnlyList<TimeSpan> delays, TimeSpan? timeout, CancellationToken cancellationToken);

    /// <summary>
    /// Builds a transport retry policy for the specified URL, retry delays, and exception predicate.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation being protected.</typeparam>
    /// <param name="url">The target URL for the operation.  Must not be <see langword="null"/>.</param>
    /// <param name="delays">The sequence of retry delays to use.  Must not be <see langword="null"/> or empty.</param>
    /// <param name="predicate">A predicate that determines which exceptions are handled by the retry policy.  Must not be
    /// <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="AsyncRetryPolicy{TResult}"/> that retries on HTTP and predicate-matching exceptions.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="url"/>, <paramref name="delays"/>, or <paramref name="predicate"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if <paramref name="cancellationToken"/> is cancelled before or during policy construction.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The retry policy will handle retryable <see cref="HttpRequestException"/> and predicate-matching exceptions.
    /// </para>
    /// </remarks>
    AsyncRetryPolicy<TResult> BuildTransportRetryPolicy<TResult>(
        Uri url, IReadOnlyList<TimeSpan> delays, Func<Exception, bool> predicate, CancellationToken cancellationToken);
}