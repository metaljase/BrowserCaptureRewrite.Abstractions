namespace Metalhead.BrowserCaptureRewrite.Abstractions.Connectivity;

/// <summary>
/// Represents configuration options for connectivity probing used to determine general internet access.
/// </summary>
/// <remarks>
/// <para>
/// This class is typically bound to the <c>ConnectivityProbeOptions</c> configuration section.
/// </para>
/// </remarks>
public class ConnectivityProbeOptions
{
    /// <summary>
    /// The configuration section name for connectivity probe options.
    /// </summary>
    public const string SectionName = "ConnectivityProbeOptions";

    /// <summary>
    /// Gets or sets the URL to probe for connectivity checks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is <c>http://www.msftncsi.com/ncsi.txt</c>.
    /// </para>
    /// </remarks>
    public Uri ProbeUrl { get; set; } = new Uri("http://www.msftncsi.com/ncsi.txt");

    /// <summary>
    /// Gets or sets the expected HTTP status code for a successful probe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is <c>200</c>.
    /// </para>
    /// </remarks>
    public int ExpectedStatusCode { get; set; } = 200;

    /// <summary>
    /// Gets or sets the timeout for the probe request, in milliseconds.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default is <c>3000</c> milliseconds.
    /// </para>
    /// </remarks>
    public float TimeoutMilliseconds { get; set; } = 3000;

    /// <summary>
    /// Gets the timeout for the probe request as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromMilliseconds(TimeoutMilliseconds);
}
