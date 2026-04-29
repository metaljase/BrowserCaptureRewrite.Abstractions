using Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Extension methods for adding resilience policies to <see cref="IBrowserSession"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// These methods allow you to wrap an <see cref="IBrowserSession"/> with a <see cref="ResilientBrowserSession"/>,
/// enabling retry, timeout, and other resilience strategies for navigation and capture operations.
/// </para>
/// </remarks>
public static class BrowserSessionResilienceExtensions
{
    /// <summary>
    /// Wraps the specified <see cref="IBrowserSession"/> with a <see cref="ResilientBrowserSession"/>
    /// that applies resilience policies from the given <see cref="IResiliencePolicyFactory"/>.
    /// </summary>
    /// <param name="session">The browser session to wrap.  Must not be <see langword="null"/>.</param>
    /// <param name="factory">The resilience policy factory to use.  Must not be <see langword="null"/>.</param>
    /// <returns>
    /// An <see cref="IBrowserSession"/> that applies resilience policies to all navigation and capture operations.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> or <paramref name="factory"/> is
    /// <see langword="null"/>.</exception>
    public static IBrowserSession WithResilience(this IBrowserSession session, IResiliencePolicyFactory factory)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(factory);
        return new ResilientBrowserSession(session, factory);
    }
}