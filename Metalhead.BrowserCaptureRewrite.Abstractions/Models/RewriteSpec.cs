using Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Specifies custom logic for selecting and rewriting HTTP responses during browser automation or resource capture.
/// </summary>
/// <remarks>
/// <para>
/// Implements the specification pattern for response rewriting.  Used to define advanced scenarios where only certain
/// requests should be considered for rewriting, and where the rewrite logic may be asynchronous and context-dependent.
/// </para>
/// <para>
/// <see cref="ShouldRewrite"/> determines whether a given <see cref="IRequestInfo"/> should be considered for rewriting.
/// <see cref="TryRewriteResponseAsync"/> attempts to rewrite the response and returns a <see cref="ResponseRewriteResult"/>
/// indicating the outcome.
/// </para>
/// <para>
/// All delegates must be non-<see langword="null"/>.  Exceptions thrown by delegates may abort the rewrite process.
/// </para>
/// <para>
/// Cancellation is supported for asynchronous operations via <see cref="CancellationToken"/> if implemented by the delegate.
/// </para>
/// </remarks>
/// <param name="shouldRewrite">
/// A function that determines whether a given <see cref="IRequestInfo"/> should be considered for rewriting.
/// Must not be <see langword="null"/>.
/// </param>
/// <param name="tryRewriteResponseAsync">
/// A function that attempts to rewrite the response for a given request and response pair.  Must not be
/// <see langword="null"/>.
/// Returns a <see cref="ResponseRewriteResult"/> indicating whether rewriting occurred and any new content or content type.
/// </param>
public sealed class RewriteSpec(
    Func<IRequestInfo, bool> shouldRewrite, Func<IRequestInfo, IResponseInfo, Task<ResponseRewriteResult>> tryRewriteResponseAsync)
{
    /// <summary>
    /// Gets the predicate that determines whether a request should be considered for response rewriting.
    /// </summary>
    /// <value>
    /// A non-<see langword="null"/> function that returns <see langword="true"/> if the request should be considered for
    /// rewriting; otherwise, <see langword="false"/>.
    /// </value>
    public Func<IRequestInfo, bool> ShouldRewrite { get; } = shouldRewrite
        ?? throw new ArgumentNullException(nameof(shouldRewrite));

    /// <summary>
    /// Gets the asynchronous function that attempts to rewrite the response for a given request and response pair.
    /// </summary>
    /// <value>
    /// A non-<see langword="null"/> function that returns a <see cref="ResponseRewriteResult"/> indicating the outcome of
    /// the rewrite attempt.
    /// </value>
    /// <remarks>
    /// <para>
    /// The returned <see cref="ResponseRewriteResult"/> indicates whether rewriting occurred and provides any new content
    /// or content type.
    /// </para>
    /// </remarks>
    public Func<IRequestInfo, IResponseInfo, Task<ResponseRewriteResult>> TryRewriteResponseAsync { get; } = tryRewriteResponseAsync
        ?? throw new ArgumentNullException(nameof(tryRewriteResponseAsync));
}
