namespace Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

/// <summary>
/// Specifies the status of a resource capture operation during page navigation and capture.
/// </summary>
/// <remarks>
/// <para>
/// Used to indicate whether the resource capture criteria were satisfied, not satisfied, or if the operation was interrupted by
/// a timeout or navigation change.
/// </para>
/// </remarks>
public enum CaptureStatus
{
    /// <summary>
    /// The resource capture criteria were not satisfied.
    /// </summary>
    CriteriaNotSatisfied,

    /// <summary>
    /// The resource capture criteria were satisfied.
    /// </summary>
    CriteriaSatisfied,

    /// <summary>
    /// The resource capture operation exceeded the allowed timeout before completion.
    /// </summary>
    CaptureTimeoutExceeded,

    /// <summary>
    /// The page URL changed before the resource capture operation could complete.
    /// </summary>
    UrlChangedBeforeCompletion
}
