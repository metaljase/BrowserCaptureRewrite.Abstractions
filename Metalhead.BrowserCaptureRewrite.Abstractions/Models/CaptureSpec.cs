using Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Specifies custom logic for selecting and capturing HTTP resources during browser automation.
/// </summary>
/// <remarks>
/// <para>
/// Used to define advanced resource capture scenarios, including custom filtering, transformation, and completion criteria.
/// </para>
/// <para>
/// <see cref="ShouldCapture"/> determines whether a request should be captured.  <see cref="TryCreateCapturedResourceAsync"/>
/// attempts to create a <see cref="CapturedResource"/> from a request/response pair.
/// <see cref="ShouldCompleteCapture"/> (optional) determines when capture should be considered complete.
/// </para>
/// <para>
/// All delegates must be non-<see langword="null"/> except <see cref="ShouldCompleteCapture"/>, which is optional.
/// Exceptions thrown by delegates may abort the capture process.
/// </para>
/// </remarks>
/// <param name="shouldCapture">
/// A function that determines whether a given <see cref="IRequestInfo"/> should be captured.  Must not be <see langword="null"/>.
/// </param>
/// <param name="tryCreateCapturedResourceAsync">
/// A function that attempts to create a <see cref="CapturedResource"/> from a request and response.  Must not be
/// <see langword="null"/>.  Returns <see langword="null"/> if the resource should not be captured.
/// </param>
/// <param name="shouldCompleteCapture">
/// Optional.  A function that determines whether capture is complete, based on navigation options, the list of captured
/// resources, and the current time.  May be <see langword="null"/> to use default completion behaviour.
/// </param>
public sealed class CaptureSpec(
    Func<IRequestInfo, bool> shouldCapture,
    Func<IRequestInfo, IResponseInfo, Task<CapturedResource?>> tryCreateCapturedResourceAsync,
    Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? shouldCompleteCapture = null)
{
    /// <summary>
    /// Gets the predicate that determines whether a request should be captured.
    /// </summary>
    /// <value>
    /// A non-<see langword="null"/> function that returns <see langword="true"/> if the request should be captured; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public Func<IRequestInfo, bool> ShouldCapture { get; } = shouldCapture
        ?? throw new ArgumentNullException(nameof(shouldCapture));

    /// <summary>
    /// Gets the function that attempts to create a <see cref="CapturedResource"/> from a request and response.
    /// </summary>
    /// <value>
    /// A non-<see langword="null"/> function that returns a <see cref="CapturedResource"/> if the resource should be captured,
    /// or <see langword="null"/> otherwise.
    /// </value>
    public Func<IRequestInfo, IResponseInfo, Task<CapturedResource?>> TryCreateCapturedResourceAsync { get; } = tryCreateCapturedResourceAsync
        ?? throw new ArgumentNullException(nameof(tryCreateCapturedResourceAsync));

    /// <summary>
    /// Gets the optional predicate that determines whether resource capture is complete.
    /// </summary>
    /// <value>
    /// A function that returns <see langword="true"/> if capture is complete, or <see langword="false"/> to continue
    /// capturing.  May be <see langword="null"/> to use default completion behaviour.
    /// </value>
    public Func<NavigationOptions, IReadOnlyList<CapturedResource>, DateTime, bool>? ShouldCompleteCapture { get; } = shouldCompleteCapture;
}