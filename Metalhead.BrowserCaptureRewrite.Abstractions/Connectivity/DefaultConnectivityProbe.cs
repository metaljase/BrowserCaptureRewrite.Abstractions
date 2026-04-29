namespace Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

/// <summary>
/// Default implementation of <see cref="IConnectivityProbe"/> that checks general internet connectivity.
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="IConnectivityProbe"/>.  Sends an HTTP GET request to the configured probe URL and uses
/// <see cref="ConnectivityProbeOptions"/> to determine the probe URL, expected status code, and timeout.
/// </para>
/// <para>
/// Cancellation is observed via <see cref="CancellationToken"/> but any exception — including
/// <see cref="OperationCanceledException"/> — is caught and causes the operation to return <see langword="false"/>
/// rather than propagating.
/// </para>
/// </remarks>
public sealed class DefaultConnectivityProbe(HttpClient client, ConnectivityProbeOptions options) : IConnectivityProbe
{
    /// <inheritdoc/>
    public Task<bool> HasGeneralConnectivityAsync(CancellationToken cancellationToken = default) =>
       HasGeneralConnectivityAsync(null, null, null, cancellationToken);

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// Any exception thrown during the HTTP request — including <see cref="OperationCanceledException"/> from
    /// cancellation or internal timeout — is caught and causes the method to return <see langword="false"/>.
    /// </para>
    /// </remarks>
    public async Task<bool> HasGeneralConnectivityAsync(
       Uri? probeUrl, int? expectedStatusCode, TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout ?? options.Timeout);

            var resp = await client.GetAsync(probeUrl ?? options.ProbeUrl, cts.Token).ConfigureAwait(false);
            return resp.IsSuccessStatusCode || (int)resp.StatusCode == (expectedStatusCode ?? options.ExpectedStatusCode);
        }
        catch
        {
            return false;
        }
    }
}
