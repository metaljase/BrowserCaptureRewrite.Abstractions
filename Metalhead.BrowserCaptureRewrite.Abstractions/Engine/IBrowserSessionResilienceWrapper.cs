namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Defines a contract for wrapping an <see cref="IBrowserSession"/> with resilience policies.
/// </summary>
/// <remarks>
/// <para>
/// Implementations provide a mechanism to add retry, timeout, and other resilience strategies to browser sessions by returning a
/// decorated <see cref="IBrowserSession"/>.
/// </para>
/// </remarks>
public interface IBrowserSessionResilienceWrapper
{
    /// <summary>
    /// Wraps the specified <see cref="IBrowserSession"/> with resilience policies.
    /// </summary>
    /// <param name="session">The browser session to wrap.  Must not be <see langword="null"/>.</param>
    /// <returns>
    /// An <see cref="IBrowserSession"/> that applies resilience policies to all navigation and capture operations.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> is <see langword="null"/>.</exception>
    IBrowserSession Wrap(IBrowserSession session);
}
