namespace Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

/// <summary>
/// Defines a contract for probing general internet connectivity.
/// </summary>
/// <remarks>
/// <para>
/// Implementations should check connectivity by making HTTP requests to a probe URL and validating the response.
/// </para>
/// </remarks>
public interface IConnectivityProbe
{
    /// <summary>
    /// Checks for general internet connectivity using the default probe URL, expected status code, and timeout.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    /// <see langword="true"/> if connectivity is confirmed; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> HasGeneralConnectivityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks for general internet connectivity using the specified probe URL, expected status code, and timeout.
    /// </summary>
    /// <param name="probeUrl">Optional probe URL to use.  If <see langword="null"/>, uses the implementation's default.</param>
    /// <param name="expectedStatusCode">Optional expected HTTP status code.  If <see langword="null"/>, uses the
    /// implementation's default.</param>
    /// <param name="timeout">Optional timeout for the probe request.  If <see langword="null"/>, uses the
    /// implementation's default.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    /// <see langword="true"/> if connectivity is confirmed; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> HasGeneralConnectivityAsync(
        Uri? probeUrl = null, int? expectedStatusCode = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
