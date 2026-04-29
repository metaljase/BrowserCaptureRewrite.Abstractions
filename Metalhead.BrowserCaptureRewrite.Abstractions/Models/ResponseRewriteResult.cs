namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Represents the result of an HTTP response rewrite operation, including whether rewriting occurred and any new content or
/// content type.
/// </summary>
/// <remarks>
/// <para>
/// Used to indicate the outcome of a response rewrite attempt during resource capture or browser automation.  If
/// <see cref="IsRewritten"/> is <see langword="true"/>, the response body and/or content type may be replaced with
/// <see cref="NewBody"/> and <see cref="ContentTypeOverride"/>.  If <see cref="IsRewritten"/> is <see langword="false"/>,
/// the original response should be used.
/// </para>
/// <para>
/// <see cref="NotRewritten"/> provides a standard result indicating that no rewrite was performed.
/// </para>
/// </remarks>
/// <param name="IsRewritten">
/// <see langword="true"/> if the response was rewritten; otherwise, <see langword="false"/>.
/// </param>
/// <param name="NewBody">
/// The new response body to use if rewritten, or <see langword="null"/> to leave the body unchanged.
/// </param>
/// <param name="ContentTypeOverride">
/// The new content type to use if rewritten, or <see langword="null"/> to leave the content type unchanged.
/// </param>
public readonly record struct ResponseRewriteResult(bool IsRewritten, string? NewBody, string? ContentTypeOverride)
{
    /// <summary>
    /// A <see cref="ResponseRewriteResult"/> indicating that no rewrite was performed.
    /// </summary>
    /// <remarks>
    /// <see cref="IsRewritten"/> is <see langword="false"/> and both <see cref="NewBody"/> and
    /// <see cref="ContentTypeOverride"/> are <see langword="null"/>.
    /// </remarks>
    public static readonly ResponseRewriteResult NotRewritten = new(false, null, null);
}