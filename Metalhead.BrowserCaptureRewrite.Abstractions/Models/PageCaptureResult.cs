using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Represents the result of a page capture operation, including HTML content, captured resources, and status information.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ResponseHtml"/> and <see cref="RenderedHtml"/> may be <see langword="null"/> if not available or not requested.
/// <see cref="Resources"/> is always non-<see langword="null"/> and may be empty if no resources were captured.
/// </para>
/// <para>
/// <see cref="PageLoadStatus"/> and <see cref="CaptureStatus"/> provide additional status information and may be
/// <see langword="null"/> if not set by the capture process.
/// </para>
/// </remarks>
/// <param name="ResponseHtml">
/// The raw HTML returned by the initial HTTP response, or <see langword="null"/> if not available.
/// </param>
/// <param name="RenderedHtml">
/// The final rendered HTML after page scripts and DOM updates, or <see langword="null"/> if not available.
/// </param>
/// <param name="Resources">
/// A list of resources captured during the page load; never <see langword="null"/>, but may be empty.
/// </param>
/// <param name="PageLoadStatus">The status of the page load operation, or <see langword="null"/> if not set.</param>
/// <param name="CaptureStatus">
/// The status of the resource capture operation, or <see langword="null"/> if not set.
/// </param>
public sealed record PageCaptureResult(
    string? ResponseHtml,
    string? RenderedHtml,
    IReadOnlyList<CapturedResource> Resources,
    PageLoadStatus? PageLoadStatus = null,
    CaptureStatus? CaptureStatus = null);
