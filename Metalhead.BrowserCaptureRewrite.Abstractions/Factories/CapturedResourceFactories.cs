using Metalhead.BrowserCaptureRewrite.Abstractions.Models;
using Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Factories;

/// <summary>
/// Provides factory methods for creating resource capture delegates that produce <see cref="CapturedResource"/> instances from HTTP
/// requests and responses.
/// </summary>
/// <remarks>
/// <para>
/// These factories return delegates that can be used with resource capture specifications to extract text, binary, or auto-detected content
/// from HTTP responses.
/// </para>
/// </remarks>
public static class CapturedResourceFactories
{
    /// <summary>
    /// Creates a factory delegate that captures the response body as text content.
    /// </summary>
    /// <returns>
    /// A delegate that produces a <see cref="CapturedResource"/> with text content from the response body.
    /// </returns>
    public static Func<IRequestInfo, IResponseInfo, Task<CapturedResource?>> Text() => async (req, resp) =>
        new CapturedResource(
            new Uri(req.Url),
            TextContent: await resp.GetBodyAsStringAsync().ConfigureAwait(false),
            BinaryContent: null,
            ContentType: resp.Headers.TryGetValue("content-type", out var ct) ? ct : null,
            StatusCode: resp.StatusCode,
            ResponseHeaders: resp.Headers);

    /// <summary>
    /// Creates a factory delegate that captures the response body as binary content.
    /// </summary>
    /// <returns>
    /// A delegate that produces a <see cref="CapturedResource"/> with binary content from the response body.
    /// </returns>
    public static Func<IRequestInfo, IResponseInfo, Task<CapturedResource?>> Binary() => async (req, resp) =>
        new CapturedResource(
            new Uri(req.Url),
            TextContent: null,
            BinaryContent: await resp.GetBodyAsByteArrayAsync().ConfigureAwait(false),
            ContentType: resp.Headers.TryGetValue("content-type", out var ct) ? ct : null,
            StatusCode: resp.StatusCode,
            ResponseHeaders: resp.Headers);

    /// <summary>
    /// Creates a factory delegate that captures the response body as text or binary content, based on the response content type.
    /// </summary>
    /// <returns>
    /// A delegate that produces a <see cref="CapturedResource"/> with text content if the content type is text, JSON, or XML; otherwise,
    /// binary content.
    /// </returns>
    public static Func<IRequestInfo, IResponseInfo, Task<CapturedResource?>> Auto() => async (req, resp) =>
    {
        string? ct = resp.Headers.TryGetValue("content-type", out var v) ? v : null;
        return ct is not null
            && (ct.Contains("text", StringComparison.OrdinalIgnoreCase)
                || ct.Contains("json", StringComparison.OrdinalIgnoreCase)
                || ct.Contains("xml", StringComparison.OrdinalIgnoreCase))
            ? new CapturedResource(
                new Uri(req.Url),
                TextContent: await resp.GetBodyAsStringAsync().ConfigureAwait(false),
                BinaryContent: null,
                ContentType: ct,
                StatusCode: resp.StatusCode,
                ResponseHeaders: resp.Headers)
            : new CapturedResource(
                new Uri(req.Url),
                TextContent: null,
                BinaryContent: await resp.GetBodyAsByteArrayAsync().ConfigureAwait(false),
                ContentType: ct,
                StatusCode: resp.StatusCode,
                ResponseHeaders: resp.Headers);
    };
}
