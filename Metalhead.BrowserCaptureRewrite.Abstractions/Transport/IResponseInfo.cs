namespace Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

/// <summary>
/// Represents HTTP response information for browser automation and resource capture.
/// </summary>
/// <remarks>
/// <para>
/// Implementations provide access to the response status code, headers, and body content as seen by the browser automation engine.
/// </para>
/// </remarks>
public interface IResponseInfo
{
    /// <summary>
    /// Gets the HTTP status code of the response, or <see langword="null"/> if unavailable.
    /// </summary>
    /// <value>
    /// The HTTP status code, or <see langword="null"/> if not applicable.
    /// </value>
    int? StatusCode { get; }

    /// <summary>
    /// Gets the HTTP headers received with the response.
    /// </summary>
    /// <value>
    /// A non-null, read-only dictionary of header names and values.
    /// </value>
    IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// Asynchronously retrieves the response body as a string.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation.  The task result is the response body as a string.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The returned string is decoded using the response's character encoding, if available.
    /// </para>
    /// </remarks>
    Task<string> GetBodyAsStringAsync();

    /// <summary>
    /// Asynchronously retrieves the response body as a byte array.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation.  The task result is the response body as a byte array.
    /// </returns>
    Task<byte[]> GetBodyAsByteArrayAsync();
}
