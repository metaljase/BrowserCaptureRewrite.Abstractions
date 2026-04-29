namespace Metalhead.BrowserCaptureRewrite.Abstractions.Enums;

/// <summary>
/// Specifies the reason for a browser session initialisation failure.
/// </summary>
public enum BrowserSessionInitializationFailureReason
{
    /// <summary>
    /// A general failure occurred during browser session initialisation.
    /// </summary>
    General,

    /// <summary>
    /// The required browser engine is not available on the system.
    /// </summary>
    EngineNotAvailable
}
