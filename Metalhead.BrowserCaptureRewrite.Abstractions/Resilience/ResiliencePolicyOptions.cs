namespace Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

/// <summary>
/// Provides configuration options for resilience policy retry behaviour, including customisable retry delay sequences for transport and timeout
/// failures.
/// </summary>
/// <remarks>
/// <para>
/// Provides settings consumed by <see cref="ResiliencePolicyFactoryBase"/> and related types.
/// </para>
/// <para>
/// Supports initialisation from integer seconds or <see cref="TimeSpan"/> values.  If no custom delays are provided, default retry delay sequences 
/// are used.
/// </para>
/// <para>
/// All properties are mutable to support configuration binding and validation.
/// </para>
/// </remarks>
public sealed class ResiliencePolicyOptions
{
    /// <summary>
    /// The configuration section name for binding <see cref="ResiliencePolicyOptions"/> from settings.
    /// </summary>
    public const string SectionName = "ResiliencePolicyOptions";

    /// <summary>
    /// Initialises a new instance of the <see cref="ResiliencePolicyOptions"/> class with default retry delay settings.
    /// </summary>
    public ResiliencePolicyOptions()
    {
        TransportRetryDelaysSeconds = null;
        TimeoutRetryDelaysSeconds = null;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ResiliencePolicyOptions"/> class using integer seconds for retry delays.
    /// </summary>
    /// <param name="transportRetryDelays">
    /// Optional.  The transport retry delays, in seconds.  If <see langword="null"/>, defaults are used.
    /// </param>
    /// <param name="timeoutRetryDelays">
    /// Optional.  The timeout retry delays, in seconds.  If <see langword="null"/>, defaults are used.
    /// </param>
    public ResiliencePolicyOptions(IReadOnlyList<int>? transportRetryDelays = null, IReadOnlyList<int>? timeoutRetryDelays = null)
    {
        TransportRetryDelaysSeconds = transportRetryDelays;
        TimeoutRetryDelaysSeconds = timeoutRetryDelays;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ResiliencePolicyOptions"/> class using <see cref="TimeSpan"/> values for retry delays.
    /// </summary>
    /// <param name="transportRetryDelays">
    /// Optional.  The transport retry delays as <see cref="TimeSpan"/> values.  If <see langword="null"/>, defaults are used.
    /// </param>
    /// <param name="timeoutRetryDelays">
    /// Optional.  The timeout retry delays as <see cref="TimeSpan"/> values.  If <see langword="null"/>, defaults are used.
    /// </param>
    public ResiliencePolicyOptions(IReadOnlyList<TimeSpan>? transportRetryDelays = null, IReadOnlyList<TimeSpan>? timeoutRetryDelays = null)
    {
        TransportRetryDelaysSeconds = transportRetryDelays?.Select(ts => (int)ts.TotalSeconds).ToList();
        TimeoutRetryDelaysSeconds = timeoutRetryDelays?.Select(ts => (int)ts.TotalSeconds).ToList();
    }

    /// <summary>
    /// The default sequence of retry delays for transport failures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used when <see cref="TransportRetryDelaysSeconds"/> is <see langword="null"/>.
    /// </para>
    /// </remarks>
    public static readonly IReadOnlyList<TimeSpan> DefaultTransportRetryDelays =
    [
        TimeSpan.FromSeconds(3),
        TimeSpan.FromSeconds(8),
        TimeSpan.FromSeconds(15),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(60),
        TimeSpan.FromMinutes(5)];

    /// <summary>
    /// The default sequence of retry delays for timeout failures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used when <see cref="TimeoutRetryDelaysSeconds"/> is <see langword="null"/>.
    /// </para>
    /// </remarks>
    public static readonly IReadOnlyList<TimeSpan> DefaultTimeoutRetryDelays =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(3),
        TimeSpan.FromSeconds(5)];

    /// <summary>
    /// Gets or sets the transport retry delays, in seconds.  If <see langword="null"/>, <see cref="DefaultTransportRetryDelays"/> is used.
    /// </summary>
    public IReadOnlyList<int>? TransportRetryDelaysSeconds { get; set; }

    /// <summary>
    /// Gets or sets the timeout retry delays, in seconds.  If <see langword="null"/>, <see cref="DefaultTimeoutRetryDelays"/> is used.
    /// </summary>
    public IReadOnlyList<int>? TimeoutRetryDelaysSeconds { get; set; }

    /// <summary>
    /// Gets the effective sequence of transport retry delays as <see cref="TimeSpan"/> values.
    /// </summary>
    /// <value>
    /// The transport retry delays, or <see cref="DefaultTransportRetryDelays"/> if <see cref="TransportRetryDelaysSeconds"/> is <see langword="null"/>.
    /// </value>
    public IReadOnlyList<TimeSpan> TransportRetryDelays =>
        TransportRetryDelaysSeconds?.Select(t => TimeSpan.FromSeconds(t)).ToList()
        ?? DefaultTransportRetryDelays;

    /// <summary>
    /// Gets the effective sequence of timeout retry delays as <see cref="TimeSpan"/> values.
    /// </summary>
    /// <value>
    /// The timeout retry delays, or <see cref="DefaultTimeoutRetryDelays"/> if <see cref="TimeoutRetryDelaysSeconds"/> is <see langword="null"/>.
    /// </value>
    public IReadOnlyList<TimeSpan> TimeoutRetryDelays =>
        TimeoutRetryDelaysSeconds?.Select(t => TimeSpan.FromSeconds(t)).ToList()
        ?? DefaultTimeoutRetryDelays;
}
