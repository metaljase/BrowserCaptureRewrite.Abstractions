using Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a connectivity-related failure is detected during browser automation or network operations.
/// </summary>
/// <param name="scope">The scope of the connectivity failure.</param>
/// <param name="innerException">The exception that is the cause of this exception.  Must not be <see langword="null"/>.</param>
/// <param name="message">An optional exception message.</param>
public sealed class ConnectivityException(ConnectivityScope scope, Exception innerException, string? message = null)
    : Exception(message, innerException)
{

    /// <summary>
    /// Gets the scope of the connectivity issue that caused the exception.
    /// </summary>
    public ConnectivityScope Scope { get; } = scope;
}