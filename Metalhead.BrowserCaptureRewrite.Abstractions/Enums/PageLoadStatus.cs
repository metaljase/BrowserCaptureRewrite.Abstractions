namespace Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

/// <summary>
/// Specifies the status of a page load operation during navigation and capture.
/// </summary>
public enum PageLoadStatus
{
    /// <summary>
    /// The page load completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The network idle timeout was exceeded before the page load completed.
    /// </summary>
    NetworkIdleTimeoutExceeded
}
