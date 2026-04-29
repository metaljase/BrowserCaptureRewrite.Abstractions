namespace Metalhead.BrowserCaptureRewrite.Abstractions.Models;

/// <summary>
/// Holds sign-in timing options that control how long to wait before assuming sign-in is complete and the maximum time
/// allowed for a page to load during sign-in.
/// </summary>
/// <remarks>
/// <para>
/// Both <see cref="AssumeSignedInAfterSeconds"/> and <see cref="PageLoadTimeoutSeconds"/> are stored as nullable
/// <see langword="float"/> values (in seconds) and exposed as nullable <see cref="TimeSpan"/> values via
/// <see cref="AssumeSignedInAfter"/> and <see cref="PageLoadTimeout"/> respectively.  A <see langword="null"/> value
/// indicates that the corresponding default behaviour should be used.
/// </para>
/// <para>
/// This class can be bound directly from configuration using the section name defined by <see cref="SectionName"/>.
/// </para>
/// </remarks>
public sealed class SignInOptions
{
    /// <summary>
    /// The configuration section name for binding <see cref="SignInOptions"/> from settings.
    /// </summary>
    public const string SectionName = "SignInOptions";

    /// <summary>
    /// Initialises a new instance with both timing values set to <see langword="null"/>.
    /// </summary>
    public SignInOptions()
    {
        AssumeSignedInAfterSeconds = null;
        PageLoadTimeoutSeconds = null;
    }

    /// <summary>
    /// Initialises a new instance with timing values specified in seconds.
    /// </summary>
    /// <param name="assumeSignedInAfterSeconds">
    /// The number of seconds to wait before assuming sign-in is complete, or <see langword="null"/> to use the default
    /// behaviour.  Must be greater than 0 if specified.
    /// </param>
    /// <param name="pageLoadTimeoutSeconds">
    /// The maximum number of seconds to wait for a page to load during sign-in, or <see langword="null"/> to use the
    /// default behaviour.  Must be greater than 0 if specified.
    /// </param>
    public SignInOptions(float? assumeSignedInAfterSeconds = null, float? pageLoadTimeoutSeconds = null)
    {
        AssumeSignedInAfterSeconds = assumeSignedInAfterSeconds;
        PageLoadTimeoutSeconds = pageLoadTimeoutSeconds;
    }

    /// <summary>
    /// Initialises a new instance with timing values specified as <see cref="TimeSpan"/> durations, which are truncated
    /// to whole seconds.
    /// </summary>
    /// <param name="assumeSignedInAfter">
    /// The duration to wait before assuming sign-in is complete, or <see langword="null"/> to use the default behaviour.
    /// Must represent a positive duration if specified.
    /// </param>
    /// <param name="pageLoadTimeout">
    /// The maximum duration to wait for a page to load during sign-in, or <see langword="null"/> to use the default
    /// behaviour.  Must represent a positive duration if specified.
    /// </param>
    public SignInOptions(TimeSpan? assumeSignedInAfter = null, TimeSpan? pageLoadTimeout = null)
    {
        AssumeSignedInAfterSeconds = assumeSignedInAfter.HasValue ? (int)assumeSignedInAfter.Value.TotalSeconds : null;
        PageLoadTimeoutSeconds = pageLoadTimeout.HasValue ? (int)pageLoadTimeout.Value.TotalSeconds : null;
    }

    /// <summary>
    /// The default value returned by <see cref="AssumeSignedInAfter"/> when <see cref="AssumeSignedInAfterSeconds"/> is
    /// <see langword="null"/>.
    /// </summary>
    public static readonly TimeSpan? DefaultAssumeSignedInAfter = null;

    /// <summary>
    /// The default value returned by <see cref="PageLoadTimeout"/> when <see cref="PageLoadTimeoutSeconds"/> is
    /// <see langword="null"/>.
    /// </summary>
    public static readonly TimeSpan? DefaultPageLoadTimeout = null;

    /// <summary>
    /// Gets or sets the number of seconds to wait before assuming sign-in is complete.  Must be greater than 0 if not
    /// <see langword="null"/>.
    /// </summary>
    public float? AssumeSignedInAfterSeconds { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of seconds to wait for a page to load during sign-in.  Must be greater than 0 if
    /// not <see langword="null"/>.
    /// </summary>
    public float? PageLoadTimeoutSeconds { get; set; }

    /// <summary>
    /// Returns the assume-signed-in-after duration as a <see cref="TimeSpan"/>, or
    /// <see cref="DefaultAssumeSignedInAfter"/> when <see cref="AssumeSignedInAfterSeconds"/> is
    /// <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing <see cref="AssumeSignedInAfterSeconds"/>, or
    /// <see cref="DefaultAssumeSignedInAfter"/> if <see cref="AssumeSignedInAfterSeconds"/> is
    /// <see langword="null"/>.
    /// </returns>
    public TimeSpan? AssumeSignedInAfter() => AssumeSignedInAfterSeconds.HasValue
        ? TimeSpan.FromSeconds((double)AssumeSignedInAfterSeconds.Value)
        : DefaultAssumeSignedInAfter;

    /// <summary>
    /// Returns the page load timeout duration as a <see cref="TimeSpan"/>, or <see cref="DefaultPageLoadTimeout"/> when
    /// <see cref="PageLoadTimeoutSeconds"/> is <see langword="null"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing <see cref="PageLoadTimeoutSeconds"/>, or
    /// <see cref="DefaultPageLoadTimeout"/> if <see cref="PageLoadTimeoutSeconds"/> is <see langword="null"/>.
    /// </returns>
    public TimeSpan? PageLoadTimeout() => PageLoadTimeoutSeconds.HasValue
        ? TimeSpan.FromSeconds((double)PageLoadTimeoutSeconds.Value)
        : DefaultPageLoadTimeout;
}
