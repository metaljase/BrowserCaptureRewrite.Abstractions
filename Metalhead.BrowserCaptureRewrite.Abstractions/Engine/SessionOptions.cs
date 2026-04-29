namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Represents options for configuring a browser session, including browser settings, sign-in behaviour, and resilience.
/// </summary>
/// <param name="BrowserOptions">The browser options to use for the session.  Must not be <see langword="null"/>.</param>
/// <param name="AssumeSignedInWhenNavigatedToUrl">Optional URL that, when navigated to, indicates the user is assumed to be signed in.
/// When <see langword="null"/>, URL-based sign-in detection is not used.</param>
/// <param name="AssumeSignedInAfter">Optional duration to wait after navigation before assuming the user is signed in.
/// When <see langword="null"/>, time-based sign-in detection is not used.</param>
/// <param name="SignInPageLoadTimeout">Optional timeout for loading the sign-in page.  When <see langword="null"/>, the default page load
/// timeout applies.</param>
/// <param name="UseResilienceForSignIn">Optional value indicating whether resilience policies are applied during sign-in navigation.
/// When <see langword="null"/>, resilience is not applied.</param>
public sealed record SessionOptions(
    BrowserOptions BrowserOptions,
    Uri? AssumeSignedInWhenNavigatedToUrl = null,
    TimeSpan? AssumeSignedInAfter = null,
    TimeSpan? SignInPageLoadTimeout = null,
    bool? UseResilienceForSignIn = null);
