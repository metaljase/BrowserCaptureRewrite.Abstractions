using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Represents the result of evaluating resource capture completion criteria, including the final status.
/// </summary>
/// <remarks>
/// <para>
/// Used to indicate whether resource capture has completed according to the specified criteria, and to provide the final
/// <see cref="CaptureStatus"/>.
/// </para>
/// <para>
/// <see cref="IsComplete"/> is <see langword="true"/> if <see cref="Status"/> is any value except
/// <see cref="CaptureStatus.CriteriaNotSatisfied"/>; otherwise, <see langword="false"/>.
/// </para>
/// </remarks>
/// <param name="Status">
/// The final status of the resource capture operation.  Must be a valid <see cref="CaptureStatus"/> value.
/// </param>
public readonly record struct CaptureCompletionResult(CaptureStatus Status)
{
    /// <summary>
    /// Gets a value indicating whether the resource capture operation is considered complete.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="Status"/> is not <see cref="CaptureStatus.CriteriaNotSatisfied"/>; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsComplete => Status != CaptureStatus.CriteriaNotSatisfied;
}
