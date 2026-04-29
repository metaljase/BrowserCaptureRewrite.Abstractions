using Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a browser session fails to initialise.
/// </summary>
/// <remarks>
/// <para>
/// This exception provides details about the failure reason, whether the session was for sign-in, and an optional resolution hint.
/// </para>
/// <para>
/// The exception message is automatically constructed from the failure reason, sign-in context, and resolution hint
/// if no custom message is provided.
/// </para>
/// </remarks>
/// <param name="reason">The reason for the browser session initialisation failure.</param>
/// <param name="isSignInSession"><see langword="true"/> if the failure occurred during a sign-in session; otherwise,
/// <see langword="false"/>.</param>
/// <param name="resolutionHint">Optional hint for resolving the failure; if not <see langword="null"/> or whitespace,
/// appended to the auto-constructed message.  Defaults to <see langword="null"/>.</param>
/// <param name="message">Custom exception message; if <see langword="null"/>, the message is automatically constructed.
/// Defaults to <see langword="null"/>.</param>
/// <param name="innerException">The inner exception, or <see langword="null"/> if none.  Defaults to
/// <see langword="null"/>.</param>
public sealed class BrowserSessionInitializationException(
    BrowserSessionInitializationFailureReason reason,
    bool isSignInSession,
    string? resolutionHint = null,
    string? message = null,
    Exception? innerException = null)
    : Exception(message ?? BuildMessage(reason, isSignInSession, resolutionHint), innerException)
{
    /// <summary>
    /// Gets the reason for the browser session initialisation failure.
    /// </summary>
    public BrowserSessionInitializationFailureReason Reason { get; } = reason;

    /// <summary>
    /// Gets a value indicating whether the failure occurred during a sign-in session.
    /// </summary>
    public bool IsSignInSession { get; } = isSignInSession;

    /// <summary>
    /// Gets an optional hint or guidance for resolving the initialisation failure.
    /// </summary>
    public string? ResolutionHint { get; } = resolutionHint;

    private static string BuildMessage(BrowserSessionInitializationFailureReason reason, bool isSignInSession, string? resolutionHint)
    {
        var signInQualifier = isSignInSession ? " (sign-in required)" : string.Empty;
        var defaultMessage = reason switch
        {
            BrowserSessionInitializationFailureReason.EngineNotAvailable =>
                $"Failed to initialize browser session{signInQualifier} because the browser automation engine is not available.",
            _ =>
                $"Failed to initialize browser session{signInQualifier}."
        };
        return $"{defaultMessage}{(string.IsNullOrWhiteSpace(resolutionHint) ? string.Empty : $"  {resolutionHint}")}";
    }
}
