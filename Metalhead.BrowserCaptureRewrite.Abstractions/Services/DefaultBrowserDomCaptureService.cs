using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Services;

/// <summary>
/// Default implementation of <see cref="IBrowserDomCaptureService"/> for capturing HTML and resources from web pages using a
/// browser session.
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="IBrowserDomCaptureService"/>.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/> and may stop in-flight work.
/// </para>
/// </remarks>
public sealed class DefaultBrowserDomCaptureService : IBrowserDomCaptureService
{
    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null)
        => await NavigateAndCaptureHtmlAndResourcesResultAsync(session, navOptions, captureSpec, null, cancellationToken, captureTimingOptions)
        .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureHtmlAndResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navOptions);
        ArgumentNullException.ThrowIfNull(captureSpec);

        captureTimingOptions ??= new CaptureTimingOptions();
        var result = await session.NavigateAndCaptureResultAsync(
            PageCaptureParts.ResponseHtml | PageCaptureParts.RenderedHtml | PageCaptureParts.Resources,
            navOptions,
            captureSpec,
            rewriteSpec,
            captureTimingOptions,
            cancellationToken).ConfigureAwait(false);

        return result.CaptureStatus is null
            ? result
            : result.CaptureStatus is CaptureStatus.UrlChangedBeforeCompletion or CaptureStatus.CaptureTimeoutExceeded
                ? throw new PageCaptureIncompleteException(result.CaptureStatus.Value, navOptions.Url)
                : result.PageLoadStatus is PageLoadStatus.NetworkIdleTimeoutExceeded
                    ? throw new PageCaptureIncompleteException(result.PageLoadStatus.Value, navOptions.Url)
                    : result;
    }
}
