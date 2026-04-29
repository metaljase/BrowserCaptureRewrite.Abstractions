using Polly;
using Polly.Fallback;
using Polly.Retry;
using Polly.Wrap;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

/// <summary>
/// Provides a base implementation for factories that construct resilience policies for browser automation and HTTP operations.
/// </summary>
/// <remarks>
/// Implements <see cref="IResiliencePolicyFactory"/>.
/// <para>
/// This type supports cancellation for all policy-building operations via <see cref="CancellationToken"/>.  If cancellation is
/// requested, in-flight work is stopped and an <see cref="OperationCanceledException"/> is thrown.
/// </para>
/// <para>
/// Derived types must implement <see cref="TransportExceptionPredicate"/> to specify which exceptions are considered
/// transport-level failures for retry and fallback policies.
/// </para>
/// </remarks>
/// <param name="resiliencePolicyBuilder">
/// The builder used to construct retry and fallback policies.  Must not be <see langword="null"/>.
/// </param>
/// <param name="options">
/// Optional.  The options used to configure retry delays and other policy behaviour.  If <see langword="null"/>,
/// default options are used.
/// </param>
public abstract class ResiliencePolicyFactoryBase(
    IResiliencePolicyBuilder resiliencePolicyBuilder, ResiliencePolicyOptions? options = null)
    : IResiliencePolicyFactory
{
    private readonly ResiliencePolicyOptions _options = options ?? new ResiliencePolicyOptions();

    /// <summary>
    /// Gets a predicate that determines whether an exception is considered a transport-level failure for retry and fallback policies.
    /// </summary>
    /// <remarks>
    /// Derived types must override this property to specify which exceptions are treated as transport errors.
    /// </remarks>
    protected abstract Func<Exception, bool> TransportExceptionPredicate { get; }

    /// <summary>
    /// Gets the sequence of retry delays used for transport-level failures.
    /// </summary>
    /// <value>
    /// The list of <see cref="TimeSpan"/> values representing the delays between retries for transport exceptions.
    /// </value>
    /// <remarks>
    /// The default value is taken from <see cref="ResiliencePolicyOptions.TransportRetryDelays"/>.
    /// </remarks>
    protected virtual IReadOnlyList<TimeSpan> TransportRetryDelays => _options.TransportRetryDelays;

    /// <summary>
    /// Gets the sequence of retry delays used for timeout failures.
    /// </summary>
    /// <value>
    /// The list of <see cref="TimeSpan"/> values representing the delays between retries for timeout exceptions.
    /// </value>
    /// <remarks>
    /// The default value is taken from <see cref="ResiliencePolicyOptions.TimeoutRetryDelays"/>.
    /// </remarks>
    protected virtual IReadOnlyList<TimeSpan> TimeoutRetryDelays => _options.TimeoutRetryDelays;

    /// <summary>
    /// Builds a transport-level retry policy for the specified URL.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation being protected.</typeparam>
    /// <param name="url">The target URL for the operation.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="AsyncRetryPolicy{TResult}"/> configured to retry on transport exceptions.
    /// </returns>
    /// <remarks>
    /// The retry delays and exception predicate are determined by <see cref="TransportRetryDelays"/> and
    /// <see cref="TransportExceptionPredicate"/>.
    /// </remarks>
    protected virtual AsyncRetryPolicy<TResult> BuildTransportRetryPolicy<TResult>(Uri url, CancellationToken cancellationToken)
    {
        return resiliencePolicyBuilder.BuildTransportRetryPolicy<TResult>(
            url, TransportRetryDelays, TransportExceptionPredicate, cancellationToken);
    }

    /// <summary>
    /// Builds a timeout-level retry policy for the specified URL.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation being protected.</typeparam>
    /// <param name="url">The target URL for the operation.  Must not be <see langword="null"/>.</param>
    /// <param name="timeout">The timeout duration for the operation, or <see langword="null"/> to use the default.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="AsyncRetryPolicy{TResult}"/> configured to retry on timeout exceptions.
    /// </returns>
    /// <remarks>
    /// The retry delays are determined by <see cref="TimeoutRetryDelays"/>.
    /// </remarks>
    protected virtual AsyncRetryPolicy<TResult> BuildTimeoutRetryPolicy<TResult>(
        Uri url, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        return resiliencePolicyBuilder.BuildTimeoutRetryPolicy<TResult>(url, TimeoutRetryDelays, timeout, cancellationToken);
    }

    /// <summary>
    /// Builds a fallback policy for the specified URL.
    /// </summary>
    /// <typeparam name="TResult">The result type of the operation being protected.</typeparam>
    /// <param name="url">The target URL for the operation.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="AsyncFallbackPolicy{TResult}"/> configured to handle transport exceptions.
    /// </returns>
    /// <remarks>
    /// The exception predicate is determined by <see cref="TransportExceptionPredicate"/>.
    /// </remarks>
    protected virtual AsyncFallbackPolicy<TResult> BuildFallbackPolicy<TResult>(Uri url, CancellationToken cancellationToken)
    {
        return resiliencePolicyBuilder.BuildFallbackPolicy<TResult>(url, TransportExceptionPredicate, cancellationToken);
    }

    /// <inheritdoc/>
    public AsyncPolicyWrap<TResult> BuildResiliencePolicy<TResult>(Uri url, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(url);

        var transportPolicy = BuildTransportRetryPolicy<TResult>(url, cancellationToken);
        var timeoutPolicy = BuildTimeoutRetryPolicy<TResult>(url, timeout, cancellationToken);
        var fallbackPolicy = BuildFallbackPolicy<TResult>(url, cancellationToken);

        return Policy.WrapAsync(fallbackPolicy, transportPolicy, timeoutPolicy);
    }
}
