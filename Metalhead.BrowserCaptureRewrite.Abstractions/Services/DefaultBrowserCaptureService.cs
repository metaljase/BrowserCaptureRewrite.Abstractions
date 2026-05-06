using Metalhead.BrowserCaptureRewrite.Abstractions.Engine;
using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;
using Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;
using Metalhead.BrowserCaptureRewrite.Abstractions.Factories;
using Metalhead.BrowserCaptureRewrite.Abstractions.Helpers;
using Metalhead.BrowserCaptureRewrite.Abstractions.Models;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Services;

/// <summary>
/// Default implementation of <see cref="IBrowserCaptureService"/> for capturing resources from web pages using a browser session.
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="IBrowserCaptureService"/>.
/// </para>
/// <para>
/// Cancellation is supported via <see cref="CancellationToken"/> for all asynchronous operations.  If cancelled, operations throw
/// <see cref="OperationCanceledException"/> and may stop in-flight work.
/// </para>
/// </remarks>
public sealed class DefaultBrowserCaptureService : IBrowserCaptureService
{
    /// <inheritdoc/>
    [Obsolete("Use NavigateAndCaptureResourcesByFileExtensionAsync instead.  This method will be removed in a future major version.")]
    public Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null) =>
        NavigateAndCaptureResourcesByFileExtensionAsync(
            session, navOptions, fileExtensions, cancellationToken, rewriteSpec, shouldCompleteCapture, captureTimingOptions);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesByFileExtensionAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        var result = await NavigateAndCaptureResourcesByFileExtensionResultAsync(
            session, navOptions, fileExtensions, cancellationToken, rewriteSpec, shouldCompleteCapture, captureTimingOptions)
            .ConfigureAwait(false);

        return result.Resources;
    }

    /// <inheritdoc/>
    [Obsolete("Use NavigateAndCaptureResourcesByFileExtensionResultAsync instead.  This method will be removed in a future major version.")]
    public Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null) =>
        NavigateAndCaptureResourcesByFileExtensionResultAsync(
            session, navOptions, fileExtensions, cancellationToken, rewriteSpec, shouldCompleteCapture, captureTimingOptions);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResourcesByFileExtensionResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] fileExtensions,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navOptions);
        ArgumentNullException.ThrowIfNull(fileExtensions);

        if (fileExtensions.Length == 0)
            throw new ArgumentException("At least one file extension must be provided.", nameof(fileExtensions));

        var normalizedExtensions = FileExtensionHelper.NormalizeFileExtensions(fileExtensions);
        if (normalizedExtensions.Length == 0)
            throw new ArgumentException("No valid file extensions after normalization.", nameof(fileExtensions));

        var captureSpec = new CaptureSpec(
            req =>
            {
                try
                {
                    var u = new Uri(req.Url, UriKind.Absolute);
                    return FileExtensionHelper.HasMatchingFileExtension(u.AbsolutePath, normalizedExtensions);
                }
                catch { return false; }
            },
            CapturedResourceFactories.Auto(),
            shouldCompleteCapture);

        return await NavigateAndCaptureResourcesResultAsync(
            session, navOptions, captureSpec, rewriteSpec, cancellationToken, captureTimingOptions)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    [Obsolete("Use NavigateAndCaptureResourcesByUrlAsync instead.  This method will be removed in a future major version.")]
    public Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null) =>
        NavigateAndCaptureResourcesByUrlAsync(
            session, navOptions, urlsToCapture, cancellationToken, rewriteSpec, captureTimingOptions);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesByUrlAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        var result = await NavigateAndCaptureResourcesByUrlResultAsync(
            session, navOptions, urlsToCapture, cancellationToken, rewriteSpec, captureTimingOptions)
            .ConfigureAwait(false);

        return result.Resources;
    }

    /// <inheritdoc/>
    [Obsolete("Use NavigateAndCaptureResourcesByUrlResultAsync instead.  This method will be removed in a future major version.")]
    public Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null) =>
        NavigateAndCaptureResourcesByUrlResultAsync(
            session, navOptions, urlsToCapture, cancellationToken, rewriteSpec, captureTimingOptions);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResourcesByUrlResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        Uri[] urlsToCapture,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navOptions);
        ArgumentNullException.ThrowIfNull(urlsToCapture);

        if (urlsToCapture.Length == 0)
            throw new ArgumentException("At least one URL must be provided.", nameof(urlsToCapture));

        var captureSpec = new CaptureSpec(
            req => urlsToCapture.Contains(UriHelper.ParseAbsoluteUrl(req.Url)),
            CapturedResourceFactories.Auto(),
            (_, resources, _) => urlsToCapture.All(url => resources.Any(r => r.Url.Equals(url))));

        return await NavigateAndCaptureResourcesResultAsync(
            session, navOptions, captureSpec, rewriteSpec, cancellationToken, captureTimingOptions)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesByContentTypeAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] contentTypes,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        var result = await NavigateAndCaptureResourcesByContentTypeResultAsync(
            session, navOptions, contentTypes, cancellationToken, rewriteSpec, shouldCompleteCapture, captureTimingOptions)
            .ConfigureAwait(false);

        return result.Resources;
    }

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResourcesByContentTypeResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        string[] contentTypes,
        CancellationToken cancellationToken,
        RewriteSpec? rewriteSpec = null,
        Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navOptions);
        ArgumentNullException.ThrowIfNull(contentTypes);

        if (contentTypes.Length == 0)
            throw new ArgumentException("At least one content type must be provided.", nameof(contentTypes));

        var normalizedContentTypes = ContentTypeHelper.NormalizeContentTypes(contentTypes);
        if (normalizedContentTypes.Length == 0)
            throw new ArgumentException("No valid content types after normalization.", nameof(contentTypes));

        var captureSpec = new CaptureSpec(
            _ => true,
            async (req, resp) =>
            {
                resp.Headers.TryGetValue("content-type", out var responseContentType);
                return !ContentTypeHelper.HasMatchingContentType(responseContentType, normalizedContentTypes)
                    ? null
                    : await CapturedResourceFactories.Auto()(req, resp).ConfigureAwait(false);
            },
            shouldCompleteCapture);

        return await NavigateAndCaptureResourcesResultAsync(
            session, navOptions, captureSpec, rewriteSpec, cancellationToken, captureTimingOptions)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null) =>
        await NavigateAndCaptureResourcesAsync(
            session, navOptions, captureSpec, null, cancellationToken, captureTimingOptions)
            .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CapturedResource>> NavigateAndCaptureResourcesAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        RewriteSpec? rewriteSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null)
    {
        var result = await NavigateAndCaptureResourcesResultAsync(
            session, navOptions, captureSpec, rewriteSpec, cancellationToken, captureTimingOptions)
            .ConfigureAwait(false);

        return result.Resources;
    }

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
        IBrowserSession session,
        NavigationOptions navOptions,
        CaptureSpec captureSpec,
        CancellationToken cancellationToken,
        CaptureTimingOptions? captureTimingOptions = null) =>
        await NavigateAndCaptureResourcesResultAsync(
            session, navOptions, captureSpec, null, cancellationToken, captureTimingOptions)
            .ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<PageCaptureResult> NavigateAndCaptureResourcesResultAsync(
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
            PageCaptureParts.Resources, navOptions, captureSpec, rewriteSpec, captureTimingOptions, cancellationToken)
            .ConfigureAwait(false);

        return result.CaptureStatus is null
            ? result
            : result.CaptureStatus is CaptureStatus.UrlChangedBeforeCompletion or CaptureStatus.CaptureTimeoutExceeded
                ? throw new PageCaptureIncompleteException(result.CaptureStatus.Value, navOptions.Url)
                : result.PageLoadStatus is PageLoadStatus.NetworkIdleTimeoutExceeded
                    ? throw new PageCaptureIncompleteException(result.PageLoadStatus.Value, navOptions.Url)
                    : result;
    }
}
