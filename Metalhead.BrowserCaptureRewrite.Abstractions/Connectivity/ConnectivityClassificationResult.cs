namespace Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

/// <summary>
/// Represents the outcome of classifying whether an error or event is connectivity-related.
/// </summary>
/// <param name="IsConnectivityRelated">
/// <see langword="true"/> when the classified condition is considered connectivity-related; otherwise, <see langword="false"/>.
/// </param>
/// <param name="Scope">
/// The classified connectivity scope.  When <paramref name="IsConnectivityRelated"/> is <see langword="false"/>, callers are
/// expected to use <see cref="ConnectivityScope.Unknown"/>.
/// </param>
/// <remarks>
/// <para>
/// This type is a passive value object that carries the outcome of connectivity classification.
/// </para>
/// <para>
/// The intended semantic contract is that <see cref="ConnectivityScope.Unknown"/> represents either a non-connectivity
/// classification or a connectivity-related classification whose scope could not be determined.
/// </para>
/// <para>
/// Callers should prefer <see cref="NotConnectivityRelated"/> for non-connectivity results and the
/// <see cref="ConnectivityRelated(ConnectivityScope)"/> factory method for connectivity-related results, rather than constructing
/// instances directly.  The primary constructor does not enforce consistency between <paramref name="IsConnectivityRelated"/>
/// and <paramref name="Scope"/>.
/// </para>
/// </remarks>
public sealed record ConnectivityClassificationResult(bool IsConnectivityRelated, ConnectivityScope Scope)
{
    /// <summary>
    /// Gets a <see cref="ConnectivityClassificationResult"/> indicating that the classified condition is not connectivity-related.
    /// </summary>
    public static ConnectivityClassificationResult NotConnectivityRelated { get; } = new(false, ConnectivityScope.Unknown);

    /// <summary>
    /// Creates a <see cref="ConnectivityClassificationResult"/> indicating that the classified condition is connectivity-related
    /// with the specified scope.
    /// </summary>
    /// <param name="scope">The classified connectivity scope to associate with the result.</param>
    /// <returns>
    /// A <see cref="ConnectivityClassificationResult"/> whose <see cref="IsConnectivityRelated"/> value is <see langword="true"/>
    /// and whose <see cref="Scope"/> value is <paramref name="scope"/>.
    /// </returns>
    public static ConnectivityClassificationResult ConnectivityRelated(ConnectivityScope scope) => new(true, scope);
}
