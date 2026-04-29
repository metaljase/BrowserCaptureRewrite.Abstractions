namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Specifies timing and completion options for resource capture operations.
/// </summary>
/// <remarks>
/// <para>
/// Used to control network idle detection, overall capture timeout, and polling interval for resource capture scenarios.
/// </para>
/// <para>
/// <see cref="NetworkIdleTimeoutSeconds"/>, <see cref="CaptureTimeoutSeconds"/>, and <see cref="PollIntervalMilliseconds"/>
/// are stored as nullable numeric values and exposed as <see cref="TimeSpan"/> values via <see cref="NetworkIdleTimeout"/>,
/// <see cref="CaptureTimeout"/>, and <see cref="PollInterval"/> respectively.  A <see langword="null"/> value for
/// <see cref="NetworkIdleTimeoutSeconds"/> or <see cref="CaptureTimeoutSeconds"/> indicates the corresponding
/// feature is disabled.  A <see langword="null"/> value for <see cref="PollIntervalMilliseconds"/> causes 
/// <see cref="PollInterval"/> to return <see cref="DefaultPollInterval"/>.
/// </para>
/// <para>
/// This class can be bound directly from configuration using the section name defined by <see cref="SectionName"/>.
/// </para>
/// </remarks>
public sealed class CaptureTimingOptions
{
    /// <summary>
    /// The configuration section name for binding <see cref="CaptureTimingOptions"/> from settings.
    /// </summary>
    public const string SectionName = "CaptureTimingOptions";

    /// <summary>
    /// Initialises a new instance with all timing values set to <see langword="null"/>.
    /// </summary>
    public CaptureTimingOptions()
    {
        NetworkIdleTimeoutSeconds = null;
        CaptureTimeoutSeconds = null;
        PollIntervalMilliseconds = null;
    }

    /// <summary>
    /// Initialises a new instance with timing values specified in seconds or milliseconds.
    /// </summary>
    /// <param name="networkIdleTimeoutSeconds">
    /// The number of seconds to wait for network inactivity before considering capture complete, or <see langword="null"/> to
    /// disable network idle detection.  Must be greater than 0 if specified.
    /// </param>
    /// <param name="captureTimeoutSeconds">
    /// The maximum number of seconds to allow for the capture operation, or <see langword="null"/> to enforce no overall
    /// timeout.  Must be greater than 0 if specified.
    /// </param>
    /// <param name="pollIntervalMilliseconds">
    /// The number of milliseconds between successive completion-condition polls, or <see langword="null"/> to use
    /// <see cref="DefaultPollInterval"/>.  Must be greater than 0 if specified.
    /// </param>
    public CaptureTimingOptions(float? networkIdleTimeoutSeconds = null, float? captureTimeoutSeconds = null,
        float? pollIntervalMilliseconds = null)
    {
        NetworkIdleTimeoutSeconds = networkIdleTimeoutSeconds;
        CaptureTimeoutSeconds = captureTimeoutSeconds;
        PollIntervalMilliseconds = pollIntervalMilliseconds;
    }

    /// <summary>
    /// Initialises a new instance with timing values specified as <see cref="TimeSpan"/> durations.
    /// </summary>
    /// <param name="networkIdleTimeout">
    /// The duration to wait for network inactivity before considering capture complete, or <see langword="null"/> to disable
    /// network idle detection.  Must represent a positive duration if specified.
    /// </param>
    /// <param name="captureTimeout">
    /// The maximum duration to allow for the capture operation, or <see langword="null"/> to enforce no overall timeout.
    /// Must represent a positive duration if specified.
    /// </param>
    /// <param name="pollInterval">
    /// The interval between successive completion-condition polls, or <see langword="null"/> to use
    /// <see cref="DefaultPollInterval"/>.  Must represent a positive duration if specified.
    /// </param>
    public CaptureTimingOptions(TimeSpan? networkIdleTimeout = null, TimeSpan? captureTimeout = null,
        TimeSpan? pollInterval = null)
    {
        NetworkIdleTimeoutSeconds = networkIdleTimeout.HasValue ? (float)networkIdleTimeout.Value.TotalSeconds : null;
        CaptureTimeoutSeconds = captureTimeout.HasValue ? (float)captureTimeout.Value.TotalSeconds : null;
        PollIntervalMilliseconds = pollInterval.HasValue ? (float)pollInterval.Value.TotalMilliseconds : null;
    }

    /// <summary>
    /// The default value returned by <see cref="PollInterval"/> when <see cref="PollIntervalMilliseconds"/> is
    /// <see langword="null"/>.
    /// </summary>
    public static readonly TimeSpan DefaultPollInterval = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Gets or sets the number of seconds to wait for network inactivity before considering capture complete.
    /// A <see langword="null"/> value disables network idle detection.  Must be greater than 0 if not <see langword="null"/>.
    /// </summary>
    public float? NetworkIdleTimeoutSeconds { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of seconds to allow for the capture operation.  A <see langword="null"/>
    /// value enforces no overall timeout.  Must be greater than 0 if not <see langword="null"/>.
    /// </summary>
    public float? CaptureTimeoutSeconds { get; set; }

    /// <summary>
    /// Gets or sets the number of milliseconds between successive completion-condition polls.  A <see langword="null"/> value
    /// causes <see cref="PollInterval"/> to return <see cref="DefaultPollInterval"/>.  Must be greater than 0 if not
    /// <see langword="null"/>.
    /// </summary>
    public float? PollIntervalMilliseconds { get; set; }

    /// <summary>
    /// Returns the network idle timeout duration as a nullable <see cref="TimeSpan"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing <see cref="NetworkIdleTimeoutSeconds"/>, or <see langword="null"/>
    /// if <see cref="NetworkIdleTimeoutSeconds"/> is <see langword="null"/>.
    /// </returns>
    public TimeSpan? NetworkIdleTimeout() => NetworkIdleTimeoutSeconds.HasValue
        ? TimeSpan.FromSeconds((double)NetworkIdleTimeoutSeconds.Value)
        : null;

    /// <summary>
    /// Returns the capture timeout duration as a nullable <see cref="TimeSpan"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing <see cref="CaptureTimeoutSeconds"/>, or <see langword="null"/>
    /// if <see cref="CaptureTimeoutSeconds"/> is <see langword="null"/>.
    /// </returns>
    public TimeSpan? CaptureTimeout() => CaptureTimeoutSeconds.HasValue
        ? TimeSpan.FromSeconds((double)CaptureTimeoutSeconds.Value)
        : null;

    /// <summary>
    /// Returns the poll interval as a <see cref="TimeSpan"/>, or <see cref="DefaultPollInterval"/> when
    /// <see cref="PollIntervalMilliseconds"/> is <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing <see cref="PollIntervalMilliseconds"/>, or <see cref="DefaultPollInterval"/>
    /// if <see cref="PollIntervalMilliseconds"/> is <see langword="null"/>.
    /// </returns>
    public TimeSpan PollInterval() => PollIntervalMilliseconds.HasValue
        ? TimeSpan.FromMilliseconds((double)PollIntervalMilliseconds.Value)
        : DefaultPollInterval;
}
