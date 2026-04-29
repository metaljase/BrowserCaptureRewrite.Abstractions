namespace Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

/// <summary>
/// Specifies the scope of a connectivity issue detected during network operations.
/// </summary>
public enum ConnectivityScope
{
    /// <summary>
    /// The connectivity issue is local to the environment (e.g., network interface, firewall, or local configuration).
    /// </summary>
    LocalEnvironment,

    /// <summary>
    /// The connectivity issue is with the remote site or service (e.g., server unreachable, connection refused, or remote timeout).
    /// </summary>
    RemoteSite,

    /// <summary>
    /// The connectivity issue is related to hostname resolution (e.g., DNS failure).
    /// </summary>
    HostnameResolution,

    /// <summary>
    /// The scope of the connectivity issue is unknown or could not be determined.
    /// </summary>
    Unknown
}
