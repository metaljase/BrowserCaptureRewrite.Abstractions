using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Fallback;
using Polly.Retry;

using Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;
using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

/// <summary>
/// Provides an implementation of <see cref="IResiliencePolicyBuilder"/> for constructing resilience policies for browser
/// automation and HTTP operations.
/// </summary>
/// <remarks>
/// Implements <see cref="IResiliencePolicyBuilder"/>.
/// <para>
/// All policy-building operations support cancellation via <see cref="CancellationToken"/>.  If cancellation is requested,
/// in-flight work is stopped and an <see cref="OperationCanceledException"/> is thrown.
/// </para>
/// <para>
/// Policies built by this class handle HTTP, timeout, and connectivity-related exceptions, and log retry/fallback behaviour using
/// the provided <see cref="ILogger{ResiliencePolicyBuilder}"/>.
/// </para>
/// </remarks>
public sealed class ResiliencePolicyBuilder(ILogger<ResiliencePolicyBuilder> logger, IConnectivityClassifier classifier, IConnectivityProbe probe)
    : IResiliencePolicyBuilder
{
    /// <inheritdoc/>
    public AsyncRetryPolicy<TResult> BuildTransportRetryPolicy<TResult>(
        Uri url,
        IReadOnlyList<TimeSpan> delays,
        Func<Exception, bool> predicate,
        CancellationToken cancellationToken)
    {
        return Policy<TResult>
            .Handle<HttpRequestException>(ex => HttpHelper.IsRetryableHttpRequestException(ex))
            .Or<Exception>(predicate)
            .WaitAndRetryAsync(delays,
                async (retryOutcome, retryDelay, retryAttempt, ctx) =>
                {
                    var ex = retryOutcome.Exception!;
                    var scope = await ConnectivityExceptionHelper.GetConnectivityScopeAsync(ex, classifier, probe, cancellationToken);
                    if (scope is ConnectivityScope.HostnameResolution)
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    var reason = GetConnectivityFailureReason(ex, scope);

                    logger.LogWarning(ex, "Error fetching URL{Reason}: {Url}", reason, url);

                    if (retryAttempt >= delays.Count)
                        logger.LogInformation("FINAL retry in {RetryDelay}...", HumanizeHelper.FormatDuration(retryDelay));
                    else
                        logger.LogInformation("Retrying in {RetryDelay}...", HumanizeHelper.FormatDuration(retryDelay));
                });
    }

    /// <inheritdoc/>
    public AsyncRetryPolicy<TResult> BuildTimeoutRetryPolicy<TResult>(
        Uri url,
        IReadOnlyList<TimeSpan> delays,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        return Policy<TResult>
            .Handle<TimeoutException>()
            .Or<TaskCanceledException>(ex => IsRetryableTaskCanceledException(ex, cancellationToken))
            .WaitAndRetryAsync(delays,
                async (retryOutcome, retryDelay, retryAttempt, ctx) =>
                {
                    var ex = retryOutcome.Exception!;
                    var scope = await ConnectivityExceptionHelper.GetConnectivityScopeAsync(ex, classifier, probe, cancellationToken);
                    var reason = GetConnectivityFailureReason(ex, scope);

                    if (ex is TimeoutException or TaskCanceledException)
                    {
                        if (timeout.HasValue)
                            logger.LogWarning(ex, "Timeout exceeded ({Timeout}) fetching URL{Reason}: {Url}", HumanizeHelper.FormatDuration(timeout.Value), reason, url);
                        else
                            logger.LogWarning(ex, "Timeout exceeded fetching URL{Reason}: {Url}", reason, url);
                    }
                    else
                        logger.LogWarning(ex, "Error fetching URL{Reason}: {Url}", reason, url);

                    if (retryAttempt >= delays.Count)
                        logger.LogInformation("FINAL retry in {RetryDelay}...", HumanizeHelper.FormatDuration(retryDelay));
                    else
                        logger.LogInformation("Retrying in {RetryDelay}...", HumanizeHelper.FormatDuration(retryDelay));
                });
    }

    /// <inheritdoc/>
    public AsyncFallbackPolicy<TResult> BuildFallbackPolicy<TResult>(
        Uri url,
        Func<Exception, bool> predicate,
        CancellationToken cancellationToken)
    {
        return Policy<TResult>
            .Handle<HttpRequestException>(ex => HttpHelper.IsRetryableHttpRequestException(ex))
            .Or<TimeoutException>()
            .Or<TaskCanceledException>(ex => IsRetryableTaskCanceledException(ex, cancellationToken))
            .Or<Exception>(predicate)
            .FallbackAsync(
               fallbackValue: default!,
               async retryOutcome =>
               {
                   var ex = retryOutcome.Exception!;
                   try
                   {
                       await ConnectivityExceptionHelper.ThrowIfConnectivityFailureAsync(ex, classifier, probe, cancellationToken)
                           .ConfigureAwait(false);
                   }
                   catch (Exception exception) when (exception is ConnectivityException or HttpRequestException)
                   {
                       exception.Data["Url"] = url.ToString();
                       throw;
                   }

                   ex.Data["Url"] = url.ToString();
                   ExceptionDispatchInfo.Capture(ex).Throw();
               });
    }

    private static bool IsRetryableTaskCanceledException(TaskCanceledException ex, CancellationToken callerToken)
    {
        if (callerToken.IsCancellationRequested)
            return false;

        var exceptionToken = ex.CancellationToken;
        return !exceptionToken.CanBeCanceled || !exceptionToken.IsCancellationRequested;
    }

    private static string GetConnectivityFailureReason(Exception ex, ConnectivityScope scope)
    {
        var scopeText = scope switch
        {
            ConnectivityScope.LocalEnvironment => "local",
            ConnectivityScope.RemoteSite => "remote",
            ConnectivityScope.HostnameResolution => "hostname",
            _ => ""
        };

        var reasonDetail = scope is ConnectivityScope.HostnameResolution
            ? $"{scopeText} related [check URL]"
            : $"{scopeText} connectivity related";
        var reason = ex is HttpRequestException hre && hre.StatusCode.HasValue
            ? $" due to {(int)hre.StatusCode.Value} {HumanizeHelper.HumanizeEnum(hre.StatusCode.Value)} ({reasonDetail})"
            : !string.IsNullOrWhiteSpace(scopeText) ? $" (appears {reasonDetail})" : "";

        return reason;
    }
}