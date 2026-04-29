namespace Metalhead.BrowserCaptureRewrite.Abstractions.Transport;

/// <summary>
/// Represents HTTP request information for browser automation and resource capture.
/// </summary>
/// <remarks>
/// <para>
/// Implementations provide access to the request URL, HTTP method, and headers as seen by the browser automation engine.
/// </para>
/// <para>
/// All properties are non-nullable and must return valid values for the underlying request.
/// </para>
/// </remarks>
public interface IRequestInfo
{
    /// <summary>
    /// Gets the absolute URL of the HTTP request.
    /// </summary>
    /// <value>
    /// The request URL as a non-null, absolute string.
    /// </value>
    string Url { get; }

    /// <summary>
    /// Gets the HTTP method used for the request (e.g., "GET", "POST").
    /// </summary>
    /// <value>
    /// The HTTP method as a non-null, upper-case string.
    /// </value>
    string Method { get; }

    /// <summary>
    /// Gets the HTTP headers sent with the request.
    /// </summary>
    /// <value>
    /// A non-null, read-only dictionary of header names and values.
    /// </value>
    IReadOnlyDictionary<string, string> Headers { get; }
}
