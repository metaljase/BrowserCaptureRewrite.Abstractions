using Metalhead.BrowserCaptureRewrite.Abstractions.Resilience;

namespace Metalhead.BrowserCaptureRewrite.Abstractions.Engine;

/// <summary>
/// Default implementation of <see cref="IBrowserSessionResilienceWrapper"/> that wraps a browser session with a
/// <see cref="ResilientBrowserSession"/> using the provided resilience policy factory.
/// </summary>
/// <remarks>
/// <para>
/// Implements <see cref="IBrowserSessionResilienceWrapper"/>.  All navigation and capture operations on the wrapped session are
/// executed with resilience policies (e.g., retry, timeout) as configured by the associated <see cref="IResiliencePolicyFactory"/>.
/// </para>
/// </remarks>
/// <param name="factory">The resilience policy factory used to create resilience policies for the wrapped session.</param>
public class DefaultBrowserSessionResilienceWrapper(IResiliencePolicyFactory factory) : IBrowserSessionResilienceWrapper
{
    /// <inheritdoc/>
    public IBrowserSession Wrap(IBrowserSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return new ResilientBrowserSession(session, factory);
    }
}
