namespace Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

/// <summary>
/// Defines a contract for classifying exceptions as connectivity-related and determining their scope.
/// </summary>
/// <remarks>
/// Implementations should analyse exceptions to determine if they are caused by connectivity issues and, if so,
/// classify the scope (e.g., local environment, remote site, hostname resolution, or unknown).
/// </remarks>
public interface IConnectivityClassifier
{
    /// <summary>
    /// Classifies the specified exception as connectivity-related or not, and determines its scope.
    /// </summary>
    /// <param name="ex">The exception to classify.  Must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">
    /// Used to distinguish a <see cref="TaskCanceledException"/> caused by a timeout from one caused by explicit
    /// cancellation.  Not used for async cancellation.
    /// </param>
    /// <returns>
    /// A <see cref="ConnectivityClassificationResult"/> indicating whether the exception is connectivity-related and its scope.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is <see langword="null"/>.</exception>
    ConnectivityClassificationResult ClassifyException(Exception ex, CancellationToken cancellationToken);
}
