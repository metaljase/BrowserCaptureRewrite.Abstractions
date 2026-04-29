using Polly.Wrap;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

/// <summary>
/// Defines a factory for constructing composite resilience policies for browser automation and HTTP operations.
/// </summary>
/// <remarks>
/// Implementations must provide a policy that combines retry, timeout, and fallback behaviour for the specified operation.
/// <para>
/// All policy-building operations must support cancellation via <see cref="CancellationToken"/>.  If cancellation is requested,
/// in-flight work must be stopped and an <see cref="OperationCanceledException"/> thrown.
/// </para>
/// </remarks>
public interface IResiliencePolicyFactory
{
    /// <summary>
    /// Builds a composite resilience policy for the specified URL, combining fallback, transport retry, and timeout
    /// retry policies.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation being protected.</typeparam>
    /// <param name="url">The target URL for the operation.  Must not be <see langword="null"/>.</param>
    /// <param name="timeout">The timeout duration for the operation, or <see langword="null"/> to use the default.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="AsyncPolicyWrap{TResult}"/> that applies fallback, transport retry, and timeout retry policies in order.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="url"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if <paramref name="cancellationToken"/> is cancelled before or during policy construction.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The returned policy will first apply fallback, then transport retry, then timeout retry.
    /// All operations support cancellation.
    /// </para>
    /// </remarks>
    AsyncPolicyWrap<TResult> BuildResiliencePolicy<TResult>(Uri url, TimeSpan? timeout, CancellationToken cancellationToken);
}
