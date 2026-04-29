namespace Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when the required browser automation engine is not installed or available.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="ResolutionHint"/> property provides optional guidance for resolving the issue.
/// </para>
/// </remarks>
/// <param name="resolutionHint">An optional hint for resolving the browser engine availability issue.</param>
/// <param name="message">An optional exception message.  Defaults to <see cref="DefaultMessage"/> if <see langword="null"/>.</param>
/// <param name="innerException">An optional inner exception.</param>
public sealed class BrowserAutomationEngineNotAvailableException(
    string? resolutionHint = null, string? message = null, Exception? innerException = null)
    : Exception(message ?? DefaultMessage, innerException)
{
    /// <summary>
    /// The default exception message used when no custom message is provided.
    /// </summary>
    public const string DefaultMessage = "Browser automation engine is not installed.  Complete the installation and try again.";

    /// <summary>
    /// Gets a hint or guidance for resolving the browser engine availability issue, or <see langword="null"/> if none was provided.
    /// </summary>
    public string? ResolutionHint { get; } = resolutionHint;
}
