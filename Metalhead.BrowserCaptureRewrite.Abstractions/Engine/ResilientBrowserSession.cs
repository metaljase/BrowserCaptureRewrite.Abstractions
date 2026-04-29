using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;
using Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// A resilient implementation of <see cref="IBrowserSession"/> that wraps another browser session and applies resilience policies (such as retry
/// and timeout) to all navigation and capture operations.
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="IBrowserSession"/>.
/// </para>
/// <para>
/// All navigation and capture operations are executed with a resilience policy as configured by the associated
/// <see cref="IResiliencePolicyFactory"/>.  This may include retries, timeouts, or other resilience strategies.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/> and may stop in-flight work.
/// </para>
/// </remarks>
/// <param name="browserSession">The underlying browser session to wrap.  Must not be <see langword="null"/>.</param>
/// <param name="resiliencePolicyFactory">The factory used to build resilience policies for navigation and capture operations.
/// Must not be <see langword="null"/>.</param>
public sealed class ResilientBrowserSession(IBrowserSession browserSession, IResiliencePolicyFactory resiliencePolicyFactory) : IBrowserSession
{
    private bool _disposed;

    /// <inheritdoc/>
    public Task<PageCaptureResult> NavigateAndCaptureResultAsync(
        PageCaptureParts captureParts,
        NavigationOptions navOptions,
        CaptureSpec? captureSpec,
        CaptureTimingOptions captureTimingOptions,
        CancellationToken cancellationToken) =>
        NavigateAndCaptureResultAsync(captureParts, navOptions, captureSpec, null, captureTimingOptions, cancellationToken);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResultAsync(
        PageCaptureParts captureParts,
        NavigationOptions navOptions,
        CaptureSpec? captureSpec,
        RewriteSpec? rewriteSpec,
        CaptureTimingOptions captureTimingOptions,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(navOptions);
        ArgumentNullException.ThrowIfNull(navOptions.Url);

        var policy = resiliencePolicyFactory.BuildResiliencePolicy<PageCaptureResult>(navOptions.Url, navOptions.PageLoadTimeout, cancellationToken);

        return await policy.ExecuteAsync(
            ct => browserSession.NavigateAndCaptureResultAsync(captureParts, navOptions, captureSpec, rewriteSpec, captureTimingOptions, ct),
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        _disposed = true;

        await browserSession.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        browserSession.Dispose();
    }
}
