using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Services;

/// <summary>
/// Default implementation of <see cref="IBrowserDomService"/> for capturing response and rendered HTML from web pages using a
/// browser session.
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="IBrowserDomService"/>.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/> and may stop in-flight work.
/// </para>
/// </remarks>
public sealed class DefaultBrowserDomService : IBrowserDomService
{
    /// <inheritdoc/>
    public async Task<string?> NavigateAndCaptureResponseHtmlAsync(
        IBrowserSession session, NavigationOptions navOptions, CancellationToken cancellationToken = default)
    {
        var result = await NavigateAndCaptureResponseHtmlResultAsync(session, navOptions, null, cancellationToken)
            .ConfigureAwait(false);
        return result.ResponseHtml;
    }

    /// <inheritdoc/>
    public async Task<string?> NavigateAndCaptureResponseHtmlAsync(
        IBrowserSession session, NavigationOptions navOptions, RewriteSpec? rewriteSpec, CancellationToken cancellationToken = default)
    {
        var result = await NavigateAndCaptureResponseHtmlResultAsync(session, navOptions, rewriteSpec, cancellationToken)
            .ConfigureAwait(false);
        return result.ResponseHtml;
    }

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResponseHtmlResultAsync(
        IBrowserSession session, NavigationOptions navOptions, CancellationToken cancellationToken = default)
        => await NavigateAndCaptureResponseHtmlResultAsync(session, navOptions, null, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResponseHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navOptions);

        var captureTimingOptions = new CaptureTimingOptions();
        return await session.NavigateAndCaptureResultAsync(
            captureParts: PageCaptureParts.ResponseHtml,
            navOptions: navOptions,
            captureSpec: null,
            rewriteSpec: rewriteSpec,
            captureTimingOptions: captureTimingOptions,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<string?> NavigateAndCaptureRenderedHtmlAsync(
        IBrowserSession session, NavigationOptions navOptions, TimeSpan? networkIdleTimeout = null, CancellationToken cancellationToken = default)
        => await NavigateAndCaptureRenderedHtmlAsync(session, navOptions, networkIdleTimeout, null, cancellationToken)
        .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<string?> NavigateAndCaptureRenderedHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default)
    {
        var result = await NavigateAndCaptureRenderedHtmlResultAsync(session, navOptions, networkIdleTimeout, rewriteSpec, cancellationToken)
            .ConfigureAwait(false);
        return result.RenderedHtml;
    }

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureRenderedHtmlResultAsync(
        IBrowserSession session, NavigationOptions navOptions, TimeSpan? networkIdleTimeout = null, CancellationToken cancellationToken = default)
        => await NavigateAndCaptureRenderedHtmlResultAsync(session, navOptions, networkIdleTimeout, null, cancellationToken)
        .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureRenderedHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navOptions);

        var captureTimingOptions = new CaptureTimingOptions(networkIdleTimeout, pollInterval: CaptureTimingOptions.DefaultPollInterval);
        var result = await session.NavigateAndCaptureResultAsync(
            captureParts: PageCaptureParts.RenderedHtml,
            navOptions: navOptions,
            captureSpec: null,
            rewriteSpec: rewriteSpec,
            captureTimingOptions: captureTimingOptions,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.PageLoadStatus is PageLoadStatus.NetworkIdleTimeoutExceeded
            ? throw new PageCaptureIncompleteException(result.PageLoadStatus.Value, navOptions.Url)
            : result;
    }

    /// <inheritdoc/>
    public async Task<(string? ResponseHtml, string? RenderedHtml)> NavigateAndCaptureResponseAndRenderedHtmlAsync(
        IBrowserSession session, NavigationOptions navOptions, TimeSpan? networkIdleTimeout = null, CancellationToken cancellationToken = default)
        => await NavigateAndCaptureResponseAndRenderedHtmlAsync(session, navOptions, networkIdleTimeout, null, cancellationToken)
        .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<(string? ResponseHtml, string? RenderedHtml)> NavigateAndCaptureResponseAndRenderedHtmlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default)
    {
        var result = await NavigateAndCaptureResponseAndRenderedHtmlResultAsync(session, navOptions, networkIdleTimeout, rewriteSpec, cancellationToken)
            .ConfigureAwait(false);
        return (result.ResponseHtml, result.RenderedHtml);
    }

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResponseAndRenderedHtmlResultAsync(
        IBrowserSession session, NavigationOptions navOptions, TimeSpan? networkIdleTimeout = null, CancellationToken cancellationToken = default)
        => await NavigateAndCaptureResponseAndRenderedHtmlResultAsync(session, navOptions, networkIdleTimeout, null, cancellationToken)
        .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResponseAndRenderedHtmlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        TimeSpan? networkIdleTimeout,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navOptions);

        var captureTimingOptions = new CaptureTimingOptions(networkIdleTimeout, pollInterval: CaptureTimingOptions.DefaultPollInterval);
        var result = await session.NavigateAndCaptureResultAsync(
            captureParts: PageCaptureParts.ResponseHtml | PageCaptureParts.RenderedHtml,
            navOptions: navOptions,
            captureSpec: null,
            rewriteSpec: rewriteSpec,
            captureTimingOptions: captureTimingOptions,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.PageLoadStatus is PageLoadStatus.NetworkIdleTimeoutExceeded
            ? throw new PageCaptureIncompleteException(result.PageLoadStatus.Value, navOptions.Url)
            : result;
    }
}
