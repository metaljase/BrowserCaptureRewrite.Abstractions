using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a page capture operation does not complete successfully due to load or resource capture criteria
/// not being met.
/// </summary>
/// <remarks>
/// <para>
/// The exception message is automatically constructed from the status and URL if no custom message is provided.
/// </para>
/// </remarks>
public sealed class PageCaptureIncompleteException : Exception
{
    /// <summary>
    /// Gets the page load status associated with the incomplete capture, if applicable.
    /// </summary>
    public PageLoadStatus? PageLoadStatus { get; }

    /// <summary>
    /// Gets the resource capture status associated with the incomplete capture, if applicable.
    /// </summary>
    public CaptureStatus? CaptureStatus { get; }

    /// <summary>
    /// Gets the URL associated with the incomplete capture, if available.
    /// </summary>
    public Uri? Url { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="PageCaptureIncompleteException"/> class with the specified page load status and URL.
    /// </summary>
    /// <param name="status">The page load status that caused the exception.</param>
    /// <param name="url">The URL associated with the incomplete capture.</param>
    /// <param name="message">An optional custom exception message.</param>
    public PageCaptureIncompleteException(PageLoadStatus status, Uri? url, string? message = null)
        : base(message ?? BuildDefaultMessage(status, url))
    {
        PageLoadStatus = status;
        Url = url;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="PageCaptureIncompleteException"/> class with the specified resource capture status and URL.
    /// </summary>
    /// <param name="status">The resource capture status that caused the exception.</param>
    /// <param name="url">The URL associated with the incomplete capture.</param>
    /// <param name="message">An optional custom exception message.</param>
    public PageCaptureIncompleteException(CaptureStatus status, Uri? url, string? message = null)
        : base(message ?? BuildDefaultMessage(status, url))
    {
        CaptureStatus = status;
        Url = url;
    }

    private static string BuildDefaultMessage(PageLoadStatus status, Uri? url) =>
        $"Page capture incomplete: {status}{(url != null ? $" for {url}" : "")}";

    private static string BuildDefaultMessage(CaptureStatus status, Uri? url) =>
        $"Page capture incomplete: {status}{(url != null ? $" for {url}" : "")}";
}